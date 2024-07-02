using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class GetSpinsStatsModel
    {
        [Required]
        public string Game { get; set; }
        public string Wallet { get; set; }
        public bool SortByXWin { get; set; }
        public long Time { get; set; }
    }
}
