using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class IdModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        [Required]
        public string Id { get; set; }
    }
}
