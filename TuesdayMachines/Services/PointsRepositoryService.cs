using System.Collections.Concurrent;
using TuesdayMachines.Interfaces;
using MongoDB.Driver;
using TuesdayMachines.Dto;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public void AddPoints(string twitchUserId, string broadcasterAccountId, long value)
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
        }


        public PointOperationResult TakePoints(string twitchUserId, string broadcasterAccountId, long value)
        {
            var wallets = _databaseService.GetWallets();
            var _lock = _locks.GetOrAdd(broadcasterAccountId, new object());
            WalletDTO walletDTO = null;

            lock (_lock)
            {
                var filter = Builders<WalletDTO>.Filter;
                walletDTO = wallets.FindOneAndUpdate(filter.Eq(x => x.TwitchUserId, twitchUserId) & filter.Eq(x => x.BroadcasterAccountId, broadcasterAccountId) & filter.Gte(x => x.Balance, value), Builders<WalletDTO>.Update.Inc(x => x.Balance, -value), new FindOneAndUpdateOptions<WalletDTO>()
                {
                    ReturnDocument = ReturnDocument.After
                });
            }

            return new PointOperationResult()
            {
                Success = walletDTO != null,
                Balance = walletDTO != null ? walletDTO.Balance : 0
            };
        }

        public void AddPoints(List<PointModifyCommand> users, string broadcasterAccountId)
        {
            if (users.Count == 0)
                return;

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
        }

        public async Task<IAsyncCursor<WalletDTO>> GetUserWallets(string twitchUserId)
        {
            var wallets = _databaseService.GetWallets();

            return await wallets.FindAsync(x => x.TwitchUserId == twitchUserId);
        }

        public async Task<PointOperationResult> GetBalance(string twitchUserId, string broadcasterAccountId)
        {
            var wallets = _databaseService.GetWallets();
            var result = await (await wallets.FindAsync(x => x.TwitchUserId == twitchUserId && x.BroadcasterAccountId == broadcasterAccountId)).FirstOrDefaultAsync();
            if (result == null)
                return new PointOperationResult() { Success = false, Balance = 0 };

            return new PointOperationResult() { Success = true, Balance = result.Balance };
        }

        public void SetPoints(string twitchUserId, string broadcasterAccountId, long value)
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
        }

        public async Task<List<WalletDTO>> GetTopAccounts(string wallet, int limit)
        {
            var wallets = _databaseService.GetWallets();

            return await (await wallets.FindAsync(x => x.BroadcasterAccountId == wallet, new FindOptions<WalletDTO>()
            {
                Limit = limit,
                Sort = Builders<WalletDTO>.Sort.Descending(x => x.Balance)
            })).ToListAsync();
        }

        public void AddPointsToAll(string broadcasterAccountId, long value)
        {
            if (value <= 0)
            {
                return;
            }

            var wallets = _databaseService.GetWallets();
            var _lock = _locks.GetOrAdd(broadcasterAccountId, new object());
            lock (_lock)
            {
                wallets.UpdateMany(x => x.BroadcasterAccountId == broadcasterAccountId, Builders<WalletDTO>.Update.Inc(x => x.Balance, value));
            }
        }
    }
}
