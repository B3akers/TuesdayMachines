using MongoDB.Driver;
using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Services
{
    public class UserFairPlayService : IUserFairPlay
    {
        private object _locker = new object();
        private readonly DatabaseService _databaseService;
        public UserFairPlayService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<UserSeedRoundInfo> GetUserSeedRoundInfo(string accountId)
        {
            var userSeeds = _databaseService.GetUserSeeds();

            UserSeedDTO userSeedDTO;

            lock (_locker)
            {
                var seed = userSeeds.Find(x => x.AccountId == accountId).FirstOrDefault();
                if (seed == null)
                {
                    userSeedDTO = CreateNew(accountId);
                    userSeeds.InsertOne(userSeedDTO);
                }
                else
                {
                    userSeeds.UpdateOne(x => x.AccountId == accountId, Builders<UserSeedDTO>.Update.Inc(x => x.Nonce, 1));
                    userSeedDTO = seed;
                }
            }

            return Task.FromResult(new UserSeedRoundInfo()
            {
                Client = userSeedDTO.Client,
                Server = userSeedDTO.ServerSeed,
                Nonce = userSeedDTO.Nonce
            });
        }

        public Task<UserSeedDTO> GetCurrentUserSeedInfo(string accountId)
        {
            var userSeeds = _databaseService.GetUserSeeds();

            UserSeedDTO userSeedDTO;
            lock (_locker)
            {
                userSeedDTO = userSeeds.Find(x => x.AccountId == accountId).FirstOrDefault();
                if (userSeedDTO == null)
                {
                    userSeedDTO = CreateNew(accountId);
                    userSeeds.InsertOne(userSeedDTO);
                }
            }

            return Task.FromResult(userSeedDTO);
        }

        public Task<Tuple<string, string>> ChangeUserSeed(string accountId, string clientSeed)
        {
            var serverSeeds = _databaseService.GetServerSeeds();
            var userSeeds = _databaseService.GetUserSeeds();
            string currentHash = string.Empty;
            string newNextHash = string.Empty;
            lock (_locker)
            {
                var seed = userSeeds.Find(x => x.AccountId == accountId).FirstOrDefault();
                if (seed != null)
                {
                    serverSeeds.InsertOne(new ServerSeedDTO()
                    {
                        HashedKey = seed.ServerSeed.HashSHA256(),
                        Key = seed.ServerSeed,
                        CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    });

                    currentHash = seed.NextServerSeed;
                    newNextHash = GetUniqueServerSeed();
                    userSeeds.UpdateOne(x => x.AccountId == accountId, Builders<UserSeedDTO>.Update.Set(x => x.ServerSeed, seed.NextServerSeed).Set(x => x.NextServerSeed, newNextHash).Set(x => x.Client, clientSeed).Set(x => x.Nonce, 0));
                }
            }

            return Task.FromResult(new Tuple<string, string>(currentHash, newNextHash));
        }

        private UserSeedDTO CreateNew(string accountId)
        {
            var userSeedDTO = new UserSeedDTO();
            userSeedDTO.AccountId = accountId;
            userSeedDTO.Client = Randomizer.RandomString(10);
            userSeedDTO.ServerSeed = GetUniqueServerSeed();
            userSeedDTO.NextServerSeed = GetUniqueServerSeed();

            return userSeedDTO;
        }

        private string GetUniqueServerSeed()
        {
            var serverSeeds = _databaseService.GetServerSeeds();
            string serverSeed = Convert.ToHexString(Randomizer.GetBytes(32)).ToLower();
            while (serverSeeds.Find(x => x.HashedKey == serverSeed.HashSHA256()).FirstOrDefault() != null)
                serverSeed = Convert.ToHexString(Randomizer.GetBytes(32)).ToLower();

            return serverSeed;
        }

        public async Task<string> GetDecryptedServerSeed(string serverSeedHashed)
        {
            var serverSeeds = _databaseService.GetServerSeeds();
            var result = await (await serverSeeds.FindAsync(x => x.HashedKey == serverSeedHashed)).FirstOrDefaultAsync();

            return result == null ? string.Empty : result.Key;
        }
    }
}
