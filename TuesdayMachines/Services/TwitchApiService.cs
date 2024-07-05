using TuesdayMachines.Interfaces;
using TuesdayMachines.Models;
using System.Text.Json;

namespace TuesdayMachines.Services
{
    public class TwitchApiService : ITwitchApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TwitchApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<TwitchAuthResponseModel> TwitchRefreshToken(string refreshToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var values = new Dictionary<string, string>
            {
                { "client_id", _configuration["Twitch:ClientId"] },
                { "client_secret", _configuration["Twitch:ClientSecret"] },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
            };

            var content = new FormUrlEncodedContent(values);
            var message = new HttpRequestMessage();
            message.RequestUri = new Uri("https://id.twitch.tv/oauth2/token");
            message.Method = HttpMethod.Post;
            message.Content = content;

            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TwitchAuthResponseModel>(stringResponse);
        }

        public async Task<TwitchAuthResponseModel> TwitchAuthorization(string code, string redirectUrl)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var values = new Dictionary<string, string>
            {
                { "client_id", _configuration["Twitch:ClientId"] },
                { "client_secret", _configuration["Twitch:ClientSecret"] },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
            };

            var content = new FormUrlEncodedContent(values);

            var message = new HttpRequestMessage();
            message.RequestUri = new Uri("https://id.twitch.tv/oauth2/token");
            message.Method = HttpMethod.Post;
            message.Content = content;

            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TwitchAuthResponseModel>(stringResponse);
        }

        public async Task<TwitchUserResponseModel> TwitchGetUserInfo(string accessToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var message = new HttpRequestMessage();
            message.RequestUri = new Uri("https://api.twitch.tv/helix/users");
            message.Method = HttpMethod.Get;
            message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            message.Headers.TryAddWithoutValidation("Client-Id", _configuration["Twitch:ClientId"]);

            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TwitchUsersResponseModel>(stringResponse);

            return result.Data[0];
        }

        public async Task<TwitchStreamResponseModel> TwitchGetUserStremInfo(string accessToken, string userId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var message = new HttpRequestMessage();
            message.RequestUri = new Uri($"https://api.twitch.tv/helix/streams?user_id={userId}");
            message.Method = HttpMethod.Get;
            message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            message.Headers.TryAddWithoutValidation("Client-Id", _configuration["Twitch:ClientId"]);

            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TwitchStreamsResponseModel>(stringResponse);

            return (result.Data != null && result.Data.Length > 0) ? result.Data[0] : null;
        }

        public async Task<TwitchSubscriptionsResponseModel> TwitchGetSubscriptions(string accessToken, string twitchUserId, string cursor)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string parametrs = $"broadcaster_id={twitchUserId}&first=100";
            if (!string.IsNullOrEmpty(cursor))
                parametrs += $"&after={cursor}";

            var message = new HttpRequestMessage();
            message.RequestUri = new Uri($"https://api.twitch.tv/helix/subscriptions?{parametrs}");
            message.Method = HttpMethod.Get;
            message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            message.Headers.TryAddWithoutValidation("Client-Id", _configuration["Twitch:ClientId"]);

            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TwitchSubscriptionsResponseModel>(stringResponse);
        }

        public async Task<TwitchChattersResponseModel> TwitchGetChatters(string accessToken, string twitchUserId, string cursor)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string parametrs = $"broadcaster_id={twitchUserId}&moderator_id={twitchUserId}&first=1000";
            if (!string.IsNullOrEmpty(cursor))
                parametrs += $"&after={cursor}";

            var message = new HttpRequestMessage();
            message.RequestUri = new Uri($"https://api.twitch.tv/helix/chat/chatters?{parametrs}");
            message.Method = HttpMethod.Get;
            message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            message.Headers.TryAddWithoutValidation("Client-Id", _configuration["Twitch:ClientId"]);

            var response = await httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TwitchChattersResponseModel>(stringResponse);
        }
    }
}
