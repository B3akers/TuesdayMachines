using System.Text.Json.Serialization;

namespace TuesdayMachines.Models
{
    public class TwitchUserResponseModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("broadcaster_type")]
        public string BroadcasterType { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("profile_image_url")]
        public string ProfilImageUrl { get; set; }

        [JsonPropertyName("offline_image_url")]
        public string OfflineImageUrl { get; set; }

        [JsonPropertyName("view_count")]
        public long ViewCount { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    };

    public class TwitchUsersResponseModel
    {
        [JsonPropertyName("data")]
        public TwitchUserResponseModel[] Data { get; set; }
    }
}
