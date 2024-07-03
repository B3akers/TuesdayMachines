using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class DefaultGamePlayModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        [Required]
        public string Wallet { get; set; }
        public long Bet { get; set; }
    }
}
