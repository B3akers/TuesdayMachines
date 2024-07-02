using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class ChangeSeedModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(24)]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        public string ClientSeed { get; set; }
    }
}
