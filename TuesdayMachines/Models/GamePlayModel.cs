using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class GamePlayModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        [Required]
        public string Wallet { get; set; }

        [Required]
        public string GameId { get; set; }
	}
}
