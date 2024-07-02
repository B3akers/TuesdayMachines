using System.Collections.Concurrent;
using TuesdayMachines.Interfaces;
using MongoDB.Driver;
using TuesdayMachines.Dto;

namespace TuesdayMachines.Services
{
    public class PointsRepositoryService : IPointsRepository
    {
        private readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();
        private readonly DatabaseService _databaseService;

        public PointsRepositoryService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task AddPoints(string twitchUserId, string broadcasterAccountId, long value)
        {
            var wallets = _databaseService.GetWallets();
            var _lock = _locks.GetOrAdd(broadcasterAccountId, new object());
            lock (_lock)
            {
                wallets.UpdateOne(x => x.TwitchUserId == twitchUserId && x.BroadcasterAccountId == broadcasterAccountId, Builders<WalletDTO>.Update.Inc(x => x.Balance, value), new UpdateOptions()
                {
                    IsUpsert = true
                });
            }

            return Task.CompletedTask;
        }


        public Task<bool> TakePoints(string twitchUserId, string broadcasterAccountId, long value)
        {
            var wallets = _databaseService.GetWallets();
            var _lock = _locks.GetOrAdd(broadcasterAccountId, new object());
            bool success = false;
            lock (_lock)
            {
                var result = wallets.UpdateOne(x => x.TwitchUserId == twitchUserId && x.BroadcasterAccountId == broadcasterAccountId && x.Balance >= value, Builders<WalletDTO>.Update.Inc(x => x.Balance, -value));
                success = result.ModifiedCount > 0;
            }

            return Task.FromResult(success);
        }

        public Task AddPoints(List<PointModifyCommand> users, string broadcasterAccountId)
        {
            if (users.Count == 0)
                return Task.CompletedTask;

            var wallets = _databaseService.GetWallets();
            var filter = Builders<WalletDTO>.Filter;

            List<UpdateOneModel<WalletDTO>> updates = new List<UpdateOneModel<WalletDTO>>(users.Count);
            foreach (var user in users)
            {
                if (user.value <= 0)
                    continue;

                updates.Add(new UpdateOneModel<WalletDTO>(filter.Eq(x => x.TwitchUserId, user.TwitchUserId) & filter.Eq(x => x.BroadcasterAccountId, broadcasterAccountId), Builders<WalletDTO>.Update.Inc(x => x.Balance, user.value))
                {
                    IsUpsert = true
                });
            }

            var _lock = _locks.GetOrAdd(broadcasterAccountId, new object());
            lock (_lock)
            {
                wallets.BulkWrite(updates);
            }

            return Task.CompletedTask;
        }

        public async Task<IAsyncCursor<WalletDTO>> GetUserWallets(string twitchUserId)
        {
            var wallets = _databaseService.GetWallets();

            return await wallets.FindAsync(x => x.TwitchUserId == twitchUserId);
        }

        public async Task<long> GetBalance(string twitchUserId, string broadcasterAccountId)
        {
            var wallets = _databaseService.GetWallets();
            var result = await (await wallets.FindAsync(x => x.TwitchUserId == twitchUserId && x.BroadcasterAccountId == broadcasterAccountId)).FirstOrDefaultAsync();
            if (result == null)
                return 0;

            return result.Balance;
        }

        public Task SetPoints(string twitchUserId, string broadcasterAccountId, long value)
        {
            var wallets = _databaseService.GetWallets();
            var _lock = _locks.GetOrAdd(broadcasterAccountId, new object());
            lock (_lock)
            {
                wallets.UpdateOne(x => x.TwitchUserId == twitchUserId && x.BroadcasterAccountId == broadcasterAccountId, Builders<WalletDTO>.Update.Set(x => x.Balance, value), new UpdateOptions()
                {
                    IsUpsert = true
                });
            }

            return Task.CompletedTask;
        }
    }
}
