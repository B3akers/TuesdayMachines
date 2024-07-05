
using MongoDB.Driver;
using TuesdayMachines.Dto;

namespace TuesdayMachines.Services
{
    public class ConfigureMongoDbService : IHostedService
    {
        private readonly DatabaseService _databaseService;

        public ConfigureMongoDbService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var accounts = _databaseService.GetAccounts();
            await accounts.Indexes.CreateOneAsync(new CreateIndexModel<AccountDTO>(Builders<AccountDTO>.IndexKeys.Ascending(x => x.TwitchId), new CreateIndexOptions() { Unique = true }));
            await accounts.Indexes.CreateOneAsync(new CreateIndexModel<AccountDTO>(Builders<AccountDTO>.IndexKeys.Ascending(x => x.TwitchLogin)));

            var devices = _databaseService.GetDevices();
            await devices.Indexes.CreateOneAsync(new CreateIndexModel<DeviceDTO>(Builders<DeviceDTO>.IndexKeys.Ascending(x => x.AccountId)));
            await devices.Indexes.CreateOneAsync(new CreateIndexModel<DeviceDTO>(Builders<DeviceDTO>.IndexKeys.Ascending(x => x.LastUse)));
            await devices.Indexes.CreateOneAsync(new CreateIndexModel<DeviceDTO>(Builders<DeviceDTO>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Unique = true }));

            var broadcasters = _databaseService.GetBroadcasters();
            await broadcasters.Indexes.CreateOneAsync(new CreateIndexModel<BroadcasterDTO>(Builders<BroadcasterDTO>.IndexKeys.Ascending(x => x.AccountId), new CreateIndexOptions() { Unique = true }));

            var wallets = _databaseService.GetWallets();
            await wallets.Indexes.CreateOneAsync(new CreateIndexModel<WalletDTO>(Builders<WalletDTO>.IndexKeys.Ascending(x => x.TwitchUserId).Ascending(x => x.BroadcasterAccountId), new CreateIndexOptions() { Unique = true }));
            await wallets.Indexes.CreateOneAsync(new CreateIndexModel<WalletDTO>(Builders<WalletDTO>.IndexKeys.Ascending(x => x.BroadcasterAccountId).Descending(x => x.Balance)));

            var serverSeeds = _databaseService.GetServerSeeds();
            await serverSeeds.Indexes.CreateOneAsync(new CreateIndexModel<ServerSeedDTO>(Builders<ServerSeedDTO>.IndexKeys.Ascending(x => x.HashedKey), new CreateIndexOptions() { Unique = true }));

            var userSeeds = _databaseService.GetUserSeeds();
            await userSeeds.Indexes.CreateOneAsync(new CreateIndexModel<UserSeedDTO>(Builders<UserSeedDTO>.IndexKeys.Ascending(x => x.AccountId), new CreateIndexOptions() { Unique = true }));

            var spins = _databaseService.GetSpins();
            await spins.Indexes.CreateOneAsync(new CreateIndexModel<SpinDTO>(Builders<SpinDTO>.IndexKeys.Ascending(x => x.Datetime).Ascending(x => x.AccountId)));

            var spinsStats = _databaseService.GetSpinsStat();
            await spinsStats.Indexes.CreateOneAsync(new CreateIndexModel<SpinStatDTO>(Builders<SpinStatDTO>.IndexKeys.Ascending(x => x.Datetime).Ascending(x => x.Game).Descending(x => x.Win)));
            await spinsStats.Indexes.CreateOneAsync(new CreateIndexModel<SpinStatDTO>(Builders<SpinStatDTO>.IndexKeys.Ascending(x => x.Datetime).Ascending(x => x.Game).Descending(x => x.WinX)));

            await spinsStats.Indexes.CreateOneAsync(new CreateIndexModel<SpinStatDTO>(Builders<SpinStatDTO>.IndexKeys.Ascending(x => x.Datetime).Ascending(x => x.Game).Ascending(x => x.Wallet).Descending(x => x.Win)));
            await spinsStats.Indexes.CreateOneAsync(new CreateIndexModel<SpinStatDTO>(Builders<SpinStatDTO>.IndexKeys.Ascending(x => x.Datetime).Ascending(x => x.Game).Ascending(x => x.Wallet).Descending(x => x.WinX)));

            var activeGames = _databaseService.GetActiveGames();
            await activeGames.Indexes.CreateOneAsync(new CreateIndexModel<ActiveGameDTO>(Builders<ActiveGameDTO>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Unique = true }));
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
