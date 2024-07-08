using MongoDB.Driver;
using TuesdayMachines.Dto;
using TuesdayMachines.Models;

namespace TuesdayMachines.Interfaces
{
    public interface IGamesRepository
    {
        Task<List<SlotGameDTO>> GetGames(bool useCache = true);
        Task DeleteGame(string id);
        Task UpdateOrCreateGame(AddGameModel model);
    }
}
