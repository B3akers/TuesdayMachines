using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace TuesdayMachines.Services
{
    public class UserAuthenticationService : IUserAuthentication
    {
        private DatabaseService _databaseService;

        public UserAuthenticationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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

            context.Session.SetString("authTimestamp", account.AuthTimestamp.ToString());
            context.Session.SetString("userId", accountId);
        }

        public async Task<AccountDTO> GetAuthenticatedUser(HttpContext context)
        {
            DeviceDTO accountDevice = null;
            var devices = _databaseService.GetDevices();
            var userId = context.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                if (!context.Request.Cookies.TryGetValue("deviceKey", out string deviceKey))
                    return null;

                accountDevice = await(await devices.FindAsync(x => x.Key == deviceKey)).FirstOrDefaultAsync();
                if (accountDevice == null)
                    return null;

                context.Session.SetString("userId", accountDevice.AccountId);

                userId = accountDevice.AccountId;
            }

            var accounts = _databaseService.GetAccounts();
            var account = await (await accounts.FindAsync(x => x.Id == userId)).FirstOrDefaultAsync();
            if (account == null)
            {
                context.Session.Remove("userId");
                if (accountDevice != null)
                {
                    context.Response.Cookies.Delete("deviceKey");
                    await devices.DeleteOneAsync(x => x.Id == accountDevice.Id);
                }
            }
            else if (accountDevice != null)
            {
                await devices.UpdateOneAsync(x => x.Id == accountDevice.Id, Builders<DeviceDTO>.Update.Set(x => x.LastUse, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
                context.Session.SetString("authTimestamp", account.AuthTimestamp.ToString());
            }
            else
            {
                if (context.Session.GetString("authTimestamp") != account.AuthTimestamp.ToString())
                {
                    context.Session.Remove("authTimestamp");
                    context.Session.Remove("userId");
                    context.Response.Cookies.Delete("deviceKey");
                    return null;
                }
            }

            return account;
        }

        public async Task LogoutUser(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("deviceKey", out var deviceKey))
            {
                var devices = _databaseService.GetDevices();
                await devices.DeleteOneAsync(x => x.Key == deviceKey);
            }

            context.Session.Remove("authTimestamp");
            context.Session.Remove("userId");
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
