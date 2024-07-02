using MongoDB.Driver;
using TuesdayMachines.Dto;
using TuesdayMachines.Models;

namespace TuesdayMachines.Interfaces
{
    public interface IGamesRepository
    {
        Task<IAsyncCursor<SlotGameDTO>> GetGames();
        Task DeleteGame(string id);
        Task UpdateOrCreateGame(AddGameModel model);
    }
}
