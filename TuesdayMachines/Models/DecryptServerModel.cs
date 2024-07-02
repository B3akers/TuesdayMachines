using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class DecryptServerModel
    {
        [Required]
        [MinLength(64)]
        [MaxLength(64)]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string ServerSeed { get; set; }
    }
}
