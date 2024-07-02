using System.Text.Json.Serialization;

namespace TuesdayMachines.Models
{
    public class TwitchPaginationModel
    {
        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }
}
