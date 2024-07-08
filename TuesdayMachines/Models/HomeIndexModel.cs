using TuesdayMachines.Dto;

namespace TuesdayMachines.Models
{
    public class HomeIndexModel
    {
        public List<SlotGameDTO> Games { get; set; }
        public int[] GamesPlayerCount { get; set; }
        public List<BroadcasterDTO> Broadcasters { get; set; }
    }
}
