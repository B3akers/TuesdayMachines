using TuesdayMachines.Dto;

namespace TuesdayMachines.Interfaces
{
    public interface ISpinsRepository
    {
        Task AddSpinLog(SpinDTO spin);
        Task AddSpinStatLog(SpinStatDTO spin);
        Task<SpinStatDTO> GetSpinStat(string id);
        Task<List<SpinStatDTO>> GetSpinsStatsLogs(long date, string game, string wallet, int limit, bool sortByMaxX);
    }
}
