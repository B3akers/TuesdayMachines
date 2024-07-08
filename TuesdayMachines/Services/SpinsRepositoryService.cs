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

        public async Task<List<SpinStatDTO>> GetSpinsStatsLogs(long date, string game, string wallet, int limit, bool sortByMaxX)
        {
            List<SpinStatDTO> result = null;

            var sort = sortByMaxX ? Builders<SpinStatDTO>.Sort.Descending(x => x.WinX) : Builders<SpinStatDTO>.Sort.Descending(x => x.Win);

            var spins = _databaseService.GetSpinsStat();
            if (string.IsNullOrEmpty(wallet))
            {
                result = await (await spins.FindAsync(x => x.Datetime >= date && x.Game == game, new FindOptions<SpinStatDTO>()
                {
                    Limit = limit,
                    Sort = sort
                })).ToListAsync();
            }
            else
            {
                result = await (await spins.FindAsync(x => x.Datetime >= date && x.Game == game && x.Wallet == wallet, new FindOptions<SpinStatDTO>()
                {
                    Limit = limit,
                    Sort = sort
                })).ToListAsync();
            }

            return result;
        }

        public async Task<SpinStatDTO> GetSpinStat(string id)
        {
            return await (await _databaseService.GetSpinsStat().FindAsync(x => x.Id == id)).FirstOrDefaultAsync();
        }
    }
}
