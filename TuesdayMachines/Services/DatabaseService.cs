using TuesdayMachines.Dto;
using MongoDB.Driver;

namespace TuesdayMachines.Services
{
    public class DatabaseService
    {
        private MongoClient _client;
        private IMongoCollection<AccountDTO> _accounts;
        private IMongoCollection<DeviceDTO> _devices;
        private IMongoCollection<BroadcasterDTO> _broadcasters;
        private IMongoCollection<WalletDTO> _wallets;
        private IMongoCollection<UserSeedDTO> _userSeeds;
        private IMongoCollection<ServerSeedDTO> _serverSeeds;
        private IMongoCollection<SpinDTO> _spins;
        private IMongoCollection<SpinStatDTO> _spinsStat;
        private IMongoCollection<SlotGameDTO> _games;
        private IMongoCollection<ActiveGameDTO> _activeGames;

        public DatabaseService(IConfiguration configuration)
        {
            _client = new MongoClient(configuration["Mongo:ConnectionString"]);
            var mongoDatabase = _client.GetDatabase(configuration["Mongo:DatabaseName"]);

            _accounts = mongoDatabase.GetCollection<AccountDTO>("accounts");
            _devices = mongoDatabase.GetCollection<DeviceDTO>("devices");
            _broadcasters = mongoDatabase.GetCollection<BroadcasterDTO>("broadcasters");
            _wallets = mongoDatabase.GetCollection<WalletDTO>("wallets");
            _userSeeds = mongoDatabase.GetCollection<UserSeedDTO>("userseeds");
            _serverSeeds = mongoDatabase.GetCollection<ServerSeedDTO>("serverseeds");
            _spins = mongoDatabase.GetCollection<SpinDTO>("spins");
            _spinsStat = mongoDatabase.GetCollection<SpinStatDTO>("spinsstat");
            _games = mongoDatabase.GetCollection<SlotGameDTO>("games");
            _activeGames = mongoDatabase.GetCollection<ActiveGameDTO>("activegames");
        }

        public IMongoCollection<AccountDTO> GetAccounts()
        {
            return _accounts;
        }
        public IMongoCollection<ActiveGameDTO> GetActiveGames()
        {
            return _activeGames;
        }
        public IMongoCollection<SlotGameDTO> GetGames()
        {
            return _games;
        }
        public IMongoCollection<SpinStatDTO> GetSpinsStat()
        {
            return _spinsStat;
        }
        public IMongoCollection<SpinDTO> GetSpins()
        {
            return _spins;
        }
        public IMongoCollection<UserSeedDTO> GetUserSeeds()
        {
            return _userSeeds;
        }

        public IMongoCollection<ServerSeedDTO> GetServerSeeds()
        {
            return _serverSeeds;
        }

        public IMongoCollection<DeviceDTO> GetDevices()
        {
            return _devices;
        }

        public IMongoCollection<BroadcasterDTO> GetBroadcasters()
        {
            return _broadcasters;
        }

        public IMongoCollection<WalletDTO> GetWallets()
        {
            return _wallets;
        }
    }
}
