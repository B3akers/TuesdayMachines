using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class GetServerSeedModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        public string Game { get; set; }
    }
}
