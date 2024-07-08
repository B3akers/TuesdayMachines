using MongoDB.Driver;
using TuesdayMachines.Models;
using TuesdayMachines.Dto;

namespace TuesdayMachines.Interfaces
{
    public interface IBroadcastersRepository
    {
        Task<List<BroadcasterDTO>> GetBroadcasters(bool useCache = true);
        Task<BroadcasterDTO> GetBroadcasterByAccountId(string accountId);
        Task<BroadcasterDTO> CreateBroadcaster(AccountDTO account, string pointNames);
        Task<List<BroadcasterDTO>> GetBroadcasters(IEnumerable<string> broadcasterAccountIds);
        Task UpdateBroadcaster(ChangeBroadcasterSettingsModel model);
        Task DeleteBroadcaster(string id);
    }
}
