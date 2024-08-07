﻿using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;

namespace TuesdayMachines.Services
{
    public class BroadcastersRepositoryService : IBroadcastersRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly IMemoryCache _memoryCache;
        public BroadcastersRepositoryService(DatabaseService databaseService, IMemoryCache memoryCache)
        {
            _databaseService = databaseService;
            _memoryCache = memoryCache;
        }

        public async Task<BroadcasterDTO> CreateBroadcaster(AccountDTO account, string pointNames)
        {
            var broadcasters = _databaseService.GetBroadcasters();

            var result = new BroadcasterDTO();
            result.Login = account.TwitchLogin;
            result.TwitchId = account.TwitchId;
            result.AccountId = account.Id;
            result.Points = pointNames;

            await broadcasters.InsertOneAsync(result);

            var accounts = _databaseService.GetAccounts();
            await accounts.UpdateOneAsync(x => x.Id == account.Id, Builders<AccountDTO>.Update.Set(x => x.AccountType, account.AccountType | (1 << 1)));

            return result;
        }

        public async Task DeleteBroadcaster(string id)
        {
            var broadcasters = _databaseService.GetBroadcasters();

            await broadcasters.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<BroadcasterDTO> GetBroadcasterByAccountId(string accountId)
        {
            var broadcasters = _databaseService.GetBroadcasters();

            return await (await broadcasters.FindAsync(x => x.AccountId == accountId)).FirstOrDefaultAsync();
        }

        public async Task<List<BroadcasterDTO>> GetBroadcasters(bool useCache)
        {
            if (!useCache)
            {
                var result = await (await _databaseService.GetBroadcasters().FindAsync(Builders<BroadcasterDTO>.Filter.Empty)).ToListAsync();
                _memoryCache.Set("broadcastersList", result);
                return result;
            }

            return await _memoryCache.GetOrCreateAsync("broadcastersList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await (await _databaseService.GetBroadcasters().FindAsync(Builders<BroadcasterDTO>.Filter.Empty)).ToListAsync();
            });
        }

        public async Task<List<BroadcasterDTO>> GetBroadcasters(IEnumerable<string> broadcasterAccountIds)
        {
            var broadcasters = _databaseService.GetBroadcasters();
            return await (await broadcasters.FindAsync(Builders<BroadcasterDTO>.Filter.In(x => x.AccountId, broadcasterAccountIds))).ToListAsync();
        }

        public async Task UpdateBroadcaster(ChangeBroadcasterSettingsModel model)
        {
            var broadcasters = _databaseService.GetBroadcasters();

            await broadcasters.UpdateOneAsync(x => x.AccountId == model.AccountId, Builders<BroadcasterDTO>.Update.Set(x => x.Points, model.Points).Set(x => x.WatchPointsSub, model.watchPointsSub).Set(x => x.WatchPoints, model.WatchPoints));
        }
    }
}
