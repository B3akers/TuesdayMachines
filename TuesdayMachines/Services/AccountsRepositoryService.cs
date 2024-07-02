using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;
using TuesdayMachines.Utils;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Text.Json;
using TuesdayMachines.Dto;
using DnsClient;

namespace TuesdayMachines.Services
{
    public class AccountsRepositoryService : IAccountsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseService _databaseService;
        public AccountsRepositoryService(IConfiguration configuration, DatabaseService databaseService)
        {
            _configuration = configuration;
            _databaseService = databaseService;
        }

        public async Task<AccountDTO> CreateOrUpdateAccount(TwitchUserResponseModel user, TwitchAuthResponseModel token)
        {
            var accounts = _databaseService.GetAccounts();

            var result = await (await accounts.FindAsync(x => x.TwitchId == user.Id)).FirstOrDefaultAsync();
            if (result == null)
            {
                result = new AccountDTO();
                result.TwitchId = user.Id;
                result.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                result.AuthTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            var encryptionKey = _configuration["AesKey"];

            result.TwitchLogin = user.Login;
            result.EncAccessToken = token.AccessToken.Encrypt(encryptionKey);
            result.EncRefreshToken = token.RefreshToken.Encrypt(encryptionKey);

            if (string.IsNullOrEmpty(result.Id))
                await accounts.InsertOneAsync(result);

            return result;
        }

        public async Task<AccountDTO> GetAccountById(string id)
        {
            var accounts = _databaseService.GetAccounts();
            return await(await accounts.FindAsync(x => x.Id == id)).FirstOrDefaultAsync();

        }

        public async Task<AccountDTO> GetAccountByTwitchLogin(string login)
        {
            var accounts = _databaseService.GetAccounts();
            return await (await accounts.FindAsync(x => x.TwitchLogin == login)).FirstOrDefaultAsync();
        }

        public async Task UpdateAccountTokens(string id, TwitchAuthResponseModel token)
        {
            var accounts = _databaseService.GetAccounts();
            var encryptionKey = _configuration["AesKey"];

            await accounts.UpdateOneAsync(x => x.Id == id, Builders<AccountDTO>.Update
                .Set(x => x.EncAccessToken, token.AccessToken.Encrypt(encryptionKey))
                .Set(x => x.EncRefreshToken, token.RefreshToken.Encrypt(encryptionKey)));
        }
    }
}
