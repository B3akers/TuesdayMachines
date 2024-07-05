using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class AddPointsAccountWalletModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        [Required]
        public string Id { get; set; }

        [Range(0, long.MaxValue)]
        public long Points { get; set; }
    }
}
