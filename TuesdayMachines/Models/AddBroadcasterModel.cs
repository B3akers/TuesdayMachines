using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class AddBroadcasterModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        public string Login { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        public string Points { get; set; }
    }
}
