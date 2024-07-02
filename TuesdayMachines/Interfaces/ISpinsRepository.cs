using TuesdayMachines.Dto;

namespace TuesdayMachines.Interfaces
{
    public interface ISpinsRepository
    {
        Task AddSpinLog(SpinDTO spin);
        Task AddSpinStatLog(SpinStatDTO spin);
    }
}
