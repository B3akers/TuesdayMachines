using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using MongoDB.Driver;

namespace TuesdayMachines.Services
{
    public class SpinsRepositoryService : ISpinsRepository
    {
        private readonly DatabaseService _databaseService;
        public SpinsRepositoryService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task AddSpinLog(SpinDTO spin)
        {
            spin.Datetime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await _databaseService.GetSpins().InsertOneAsync(spin);
        }

        public async Task AddSpinStatLog(SpinStatDTO spin)
        {
            spin.Datetime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await _databaseService.GetSpinsStat().InsertOneAsync(spin);
        }
    }
}
