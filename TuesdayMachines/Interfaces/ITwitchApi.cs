using TuesdayMachines.Models;

namespace TuesdayMachines.Interfaces
{
    public interface ITwitchApi
    {
        Task<TwitchAuthResponseModel> TwitchAuthorization(string code, string redirectUrl);
        Task<TwitchUserResponseModel> TwitchGetUserInfo(string accessToken);
        Task<TwitchStreamResponseModel> TwitchGetUserStremInfo(string accessToken, string userId);
        Task<TwitchAuthResponseModel> TwitchRefreshToken(string refreshToken);
        Task<TwitchChattersResponseModel> TwitchGetChatters(string accessToken, string twitchUserId, string cursor);
    }
}
