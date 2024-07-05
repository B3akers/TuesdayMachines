using TuesdayMachines.Dto;

namespace TuesdayMachines.Interfaces
{
    public struct UserSeedRoundInfo
    {
        public string Client;
        public string Server;
        public long Nonce;
    };

    public interface IUserFairPlay
    {
        MinesActiveGameDTO GetMinesActiveGame(string accountId);
        void AddMinesGame(string accountId, string walletId, long bet, int[] bombs);
        void RemoveMinesGame(string accountId);
        void UpdateMinesGame(string accountId, int index);
        UserSeedRoundInfo GetUserSeedRoundInfo(string accountId);
        Task<RouletteActiveSeedDTO> GetCurrentLiveRouletteRoundInfo();
        Task<UserSeedRoundInfo> GetNextLiveRouletteRoundInfo();
        Task<UserSeedDTO> GetCurrentUserSeedInfo(string accountId);
        Task<Tuple<string, string>> ChangeUserSeed(string accountId, string clientSeed);
        Task<string> GetDecryptedServerSeed(string serverSeedHashed);
    }
}
