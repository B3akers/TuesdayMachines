using MongoDB.Driver;
using TuesdayMachines.Dto;

namespace TuesdayMachines.Interfaces
{
    public struct PointModifyCommand
    {
        public string TwitchUserId;
        public long value;
    };

    public interface IPointsRepository
    {
        Task SetPoints(string twitchUserId, string broadcasterAccountId, long value);
        Task AddPoints(string twitchUserId, string broadcasterAccountId, long value);
        Task AddPoints(List<PointModifyCommand> users, string broadcasterAccountId);
        Task<bool> TakePoints(string twitchUserId, string broadcasterAccountId, long value);
        Task<long> GetBalance(string twitchUserId, string broadcasterAccountId);
        Task<IAsyncCursor<WalletDTO>> GetUserWallets(string twitchUserId);
    }
}
