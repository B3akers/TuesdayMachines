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
        Task<UserSeedRoundInfo> GetUserSeedRoundInfo(string accountId);
        Task<UserSeedDTO> GetCurrentUserSeedInfo(string accountId);
        Task<Tuple<string, string>> ChangeUserSeed(string accountId, string clientSeed);
        Task<string> GetDecryptedServerSeed(string serverSeedHashed);
    }
}
