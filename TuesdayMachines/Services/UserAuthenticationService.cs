using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TuesdayMachines.Services
{
    public class UserAuthenticationService : IUserAuthentication
    {
        private DatabaseService _databaseService;
        private IJwtTokenHandler _jwtTokenHandler;

        public UserAuthenticationService(DatabaseService databaseService, IJwtTokenHandler jwtTokenHandler)
        {
            _databaseService = databaseService;
            _jwtTokenHandler = jwtTokenHandler;
        }

        public void RegenerateTokenForUser(HttpContext context, AccountDTO account)
        {
            var token = _jwtTokenHandler.GenerateToken(new ClaimsIdentity(new Claim[]
            {
                  new Claim("Id", account.Id),
                  new Claim("TwitchId", account.TwitchId),
                  new Claim("TwitchLogin", account.TwitchLogin),
                  new Claim("AccountType", account.AccountType.ToString()),
            }),
            DateTime.UtcNow.AddMinutes(30));

            context.Response.Cookies.Append("sessionToken", token, new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddMinutes(30) });
        }

        public async Task AuthorizeForUser(HttpContext context, string accountId, bool permanent)
        {
            var accounts = _databaseService.GetAccounts();

            var account = await (await accounts.FindAsync(x => x.Id == accountId)).FirstOrDefaultAsync();
            if (account == null)
                return;

            if (permanent)
            {
                var devices = _databaseService.GetDevices();
                var device = new DeviceDTO() { AccountId = account.Id, Key = Randomizer.RandomString(100), LastUse = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };

                await devices.InsertOneAsync(device);

                context.Response.Cookies.Append("deviceKey", device.Key, new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            }

            RegenerateTokenForUser(context, account);
        }

        public async Task<AccountDTO> GetAuthenticatedUser(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("sessionToken", out var session))
            {
                var claimsPrincipal = _jwtTokenHandler.ValidateToken(session);
                if (claimsPrincipal != null)
                {
                    AccountDTO accountDTO = new AccountDTO();

                    foreach (var claim in claimsPrincipal.Claims)
                    {
                        switch (claim.Type)
                        {
                            case "Id":
                                accountDTO.Id = claim.Value;
                                break;
                            case "TwitchId":
                                accountDTO.TwitchId = claim.Value;
                                break;
                            case "TwitchLogin":
                                accountDTO.TwitchLogin = claim.Value;
                                break;
                            case "AccountType":
                                accountDTO.AccountType = int.Parse(claim.Value);
                                break;
                        }
                    }

                    return accountDTO;
                }
            }

            if (!context.Request.Cookies.TryGetValue("deviceKey", out string deviceKey))
                return null;

            var devices = _databaseService.GetDevices();
            DeviceDTO accountDevice = await (await devices.FindAsync(x => x.Key == deviceKey)).FirstOrDefaultAsync();
            if (accountDevice == null)
                return null;

            var accounts = _databaseService.GetAccounts();
            var account = await (await accounts.FindAsync(x => x.Id == accountDevice.AccountId)).FirstOrDefaultAsync();
            if (account == null)
            {
                context.Response.Cookies.Delete("deviceKey");
                await devices.DeleteOneAsync(x => x.Id == accountDevice.Id);
                return null;
            }

            await devices.UpdateOneAsync(x => x.Id == accountDevice.Id, Builders<DeviceDTO>.Update.Set(x => x.LastUse, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));

            RegenerateTokenForUser(context, account);

            return account;
        }

        public async Task LogoutUser(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("deviceKey", out var deviceKey))
            {
                var devices = _databaseService.GetDevices();
                await devices.DeleteOneAsync(x => x.Key == deviceKey);
            }

            context.Response.Cookies.Delete("sessionToken");
            context.Response.Cookies.Delete("deviceKey");
        }

        public async Task LogoutUser(string accountId)
        {
            var devices = _databaseService.GetDevices();
            await devices.DeleteManyAsync(x => x.AccountId == accountId);

            var accounts = _databaseService.GetAccounts();
            await accounts.UpdateOneAsync(x => x.Id == accountId, Builders<AccountDTO>.Update.Set(x => x.AuthTimestamp, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        }
    }
}
