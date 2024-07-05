using System.Text.Json.Serialization;

namespace TuesdayMachines.Models
{
    public class TwitchSubscriptionResponseModel
    {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("broadcaster_login")]
        public string BroadcasterLogin { get; set; }

        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }

        [JsonPropertyName("gifter_id")]
        public string GifterId { get; set; }

        [JsonPropertyName("gifter_login")]
        public string GifterLogin { get; set; }

        [JsonPropertyName("gifter_name")]
        public string GifterName { get; set; }

        [JsonPropertyName("is_gift")]
        public bool IsGift { get; set; }

        [JsonPropertyName("tier")]
        public string Tier { get; set; }

        [JsonPropertyName("plan_name")]
        public string PlanName { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }
    }

    public class TwitchSubscriptionsResponseModel
    {
        [JsonPropertyName("data")]
        public TwitchSubscriptionResponseModel[] Data { get; set; }

        [JsonPropertyName("pagination")]
        public TwitchPaginationModel Pagination { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
