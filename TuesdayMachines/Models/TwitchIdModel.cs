using System.ComponentModel.DataAnnotations;

namespace TuesdayMachines.Models
{
    public class TwitchIdModel
    {
        [Required]
        public string TwitchId { get; set; }
    }
}
