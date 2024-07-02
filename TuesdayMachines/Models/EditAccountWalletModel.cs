using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class EditAccountWalletModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        [Required]
        public string Id { get; set; }

        [Required]
        public string TwitchId { get; set; }

        public long Points { get; set; }
    }
}
