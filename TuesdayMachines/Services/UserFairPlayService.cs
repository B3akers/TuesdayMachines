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

        public async Task<RouletteActiveSeedDTO> GetCurrentLiveRouletteRoundInfo()
        {
            var liveGames = _databaseService.GetActiveGames().OfType<RouletteActiveSeedDTO>();
            return await (await liveGames.FindAsync(x => x.Key == "roulette")).FirstOrDefaultAsync();
        }

        public async Task<UserSeedRoundInfo> GetNextLiveRouletteRoundInfo()
        {
            var liveGames = _databaseService.GetActiveGames().OfType<RouletteActiveSeedDTO>();
            var data = await (await liveGames.FindAsync(x => x.Key == "roulette")).FirstOrDefaultAsync();
            if (data == null)
            {
                RouletteActiveSeedDTO activeSeedDTO = new RouletteActiveSeedDTO();

                activeSeedDTO.Key = "roulette";
                activeSeedDTO.ClientSeed = Randomizer.RandomString(10);
                activeSeedDTO.ServerSeed = GetUniqueServerSeed();
                activeSeedDTO.NextServerSeed = GetUniqueServerSeed();
                activeSeedDTO.Nonce = 0;
                activeSeedDTO.CreateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                await liveGames.InsertOneAsync(activeSeedDTO);

                return new UserSeedRoundInfo()
                {
                    Client = activeSeedDTO.ClientSeed,
                    Server = activeSeedDTO.ServerSeed,
                    Nonce = activeSeedDTO.Nonce
                };
            }

            var delta = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - data.CreateTime;
            if (delta > TimeSpan.FromDays(1).TotalSeconds)
            {
                {
                    var serverSeeds = _databaseService.GetServerSeeds();
                    await serverSeeds.InsertOneAsync(new ServerSeedDTO()
                    {
                        HashedKey = data.ServerSeed.HashSHA256(),
                        Key = data.ServerSeed,
                        CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    });
                }

                var nextServerSeed = GetUniqueServerSeed();
                await liveGames.UpdateOneAsync(x => x.Id == data.Id, Builders<RouletteActiveSeedDTO>.Update.Set(x => x.ServerSeed, data.NextServerSeed).Set(x => x.NextServerSeed, nextServerSeed).Set(x => x.CreateTime, DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Set(x => x.Nonce, 0));

                return new UserSeedRoundInfo()
                {
                    Client = data.ClientSeed,
                    Server = data.NextServerSeed,
                    Nonce = 0
                };
            }

            await liveGames.UpdateOneAsync(x => x.Id == data.Id, Builders<RouletteActiveSeedDTO>.Update.Inc(x => x.Nonce, 1));

            return new UserSeedRoundInfo()
            {
                Client = data.ClientSeed,
                Server = data.ServerSeed,
                Nonce = data.Nonce + 1
            };
        }

        public UserSeedRoundInfo GetUserSeedRoundInfo(string accountId)
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

            return new UserSeedRoundInfo()
            {
                Client = userSeedDTO.Client,
                Server = userSeedDTO.ServerSeed,
                Nonce = userSeedDTO.Nonce
            };
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

        public MinesActiveGameDTO GetMinesActiveGame(string accountId)
        {
            var liveGames = _databaseService.GetActiveGames().OfType<MinesActiveGameDTO>();
            return liveGames.Find(x => x.Key == $"{accountId}_mines").FirstOrDefault();
        }

        public void AddMinesGame(string accountId, string walletId, long bet, int[] bombs)
        {
            var liveGames = _databaseService.GetActiveGames().OfType<MinesActiveGameDTO>();
            var game = new MinesActiveGameDTO();
            game.Key = $"{accountId}_mines";
            game.AccountId = accountId;
            game.WalletId = walletId;
            game.Picked = [];
            game.Bombs = bombs;
            game.Bet = bet;

            liveGames.InsertOne(game);
        }

        public void RemoveMinesGame(string accountId)
        {
            var liveGames = _databaseService.GetActiveGames().OfType<MinesActiveGameDTO>();
            liveGames.DeleteOne(x => x.Key == $"{accountId}_mines");
        }

        public void UpdateMinesGame(string accountId, int index)
        {
            var liveGames = _databaseService.GetActiveGames().OfType<MinesActiveGameDTO>();
            liveGames.UpdateOne(x => x.Key == $"{accountId}_mines", Builders<MinesActiveGameDTO>.Update.AddToSet(x => x.Picked, index));
        }
    }
}
