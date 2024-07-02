using System.Text.Json.Serialization;

namespace TuesdayMachines.Models
{
    public class TwitchChatterResponseModel
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
    }

    public class TwitchChattersResponseModel
    {
        [JsonPropertyName("data")]
        public TwitchChatterResponseModel[] Data { get; set; }

        [JsonPropertyName("pagination")]
        public TwitchPaginationModel Pagination { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
