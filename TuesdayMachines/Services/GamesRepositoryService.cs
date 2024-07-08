using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;

namespace TuesdayMachines.Services
{
    public class GamesRepositoryService : IGamesRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly IMemoryCache _memoryCache;
        public GamesRepositoryService(DatabaseService databaseService, IMemoryCache memoryCache)
        {
            _databaseService = databaseService;
            _memoryCache = memoryCache;
        }

        public async Task DeleteGame(string id)
        {
            await _databaseService.GetGames().DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<SlotGameDTO>> GetGames(bool useCache)
        {
            if (!useCache)
            {
                var result = await (await _databaseService.GetGames().FindAsync(Builders<SlotGameDTO>.Filter.Empty)).ToListAsync();
                _memoryCache.Set("gamesList", result);
                return result;
            }

            return await _memoryCache.GetOrCreateAsync("gamesList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await (await _databaseService.GetGames().FindAsync(Builders<SlotGameDTO>.Filter.Empty)).ToListAsync();
            });
        }

        public async Task UpdateOrCreateGame(AddGameModel model)
        {
            var games = _databaseService.GetGames();

            if (string.IsNullOrEmpty(model.Id))
            {
                var record = new SlotGameDTO();
                record.Name = model.Name;
                record.Code = model.Code;
                record.Color = model.Color;
                record.Logo = model.Logo;
                record.Metadata = model.Metadata.Split('\n').ToList();

                await games.InsertOneAsync(record);

                return;
            }

            await games.UpdateOneAsync(x => x.Id == model.Id, Builders<SlotGameDTO>.Update.Set(x => x.Name, model.Name).Set(x => x.Code, model.Code).Set(x => x.Color, model.Color).Set(x => x.Logo, model.Logo).Set(x => x.Metadata, model.Metadata.Split('\n').ToList()));
        }
    }
}
