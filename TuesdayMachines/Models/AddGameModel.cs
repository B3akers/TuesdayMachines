using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class AddGameModel
    {
        [RegularExpression("^[a-f\\d]{24}$")]
        public string Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [RegularExpression("^[a-zA-Z0-9_ ]*$")]
        public string Name { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        public string Code { get; set; }

        [Required]
        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$")]
        public string Color { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(256)]
        public string Logo { get; set; }

        [MaxLength(512)]
        public string Metadata { get; set; }
    }
}
