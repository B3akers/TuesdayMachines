using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class ChangeBroadcasterSettingsModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        [Required]
        public string AccountId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        public string Points { get; set; }

        public long WatchPoints { get; set; }
        public long watchPointsSub { get; set; }
    }
}
