
using MongoDB.Driver;
using System.Net;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Services
{
    public class WatchTimeUpdateService : BackgroundService
    {
        private readonly IBroadcastersRepository _broadcastersRepository;
        private readonly IAccountsRepository _accountRepository;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IConfiguration _configuration;
        private readonly ITwitchApi _twitchApi;
        private readonly IPointsRepository _pointsRepository;

        public WatchTimeUpdateService(IBroadcastersRepository broadcastersRepository, IConfiguration configuration, IAccountsRepository accountRepository, ITwitchApi twitchApi, IUserAuthentication userAuthentication, IPointsRepository pointsRepository)
        {
            _broadcastersRepository = broadcastersRepository;
            _configuration = configuration;
            _twitchApi = twitchApi;
            _accountRepository = accountRepository;
            _userAuthentication = userAuthentication;
            _pointsRepository = pointsRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var key = _configuration["AesKey"];

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cursor = await _broadcastersRepository.GetBroadcasters();
                    await cursor.ForEachAsync(async broadcaster =>
                    {
                        var account = await _accountRepository.GetAccountById(broadcaster.AccountId);

                        try
                        {
                            var accessToken = account.EncAccessToken.Decrypt(key);
                            if (string.IsNullOrEmpty(accessToken))
                                return;

                            var userInfo = await _twitchApi.TwitchGetUserStremInfo(accessToken, account.TwitchId);
                            if (userInfo == null || userInfo.Type != "live")
                                return;

                            if (broadcaster.WatchPoints == 0 && broadcaster.WatchPointsSub == 0)
                                return;

                            string currentCursor = string.Empty;

                            while (true)
                            {
                                var chatters = await _twitchApi.TwitchGetChatters(accessToken, account.TwitchId, currentCursor);
                                if (chatters.Data.Length == 0)
                                    break;

                                await _pointsRepository.AddPoints(chatters.Data.Select(x => new PointModifyCommand() { TwitchUserId = x.UserId, value = broadcaster.WatchPoints }).ToList(), account.Id);

                                if (chatters.Pagination == null || string.IsNullOrEmpty(chatters.Pagination.Cursor))
                                    break;

                                currentCursor = chatters.Pagination.Cursor;
                            }
                        }
                        catch (HttpRequestException exception)
                        {
                            //Try to exchange token
                            //
                            if (exception.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                try
                                {
                                    var newToken = await _twitchApi.TwitchRefreshToken(account.EncRefreshToken.Decrypt(key));
                                    await _accountRepository.UpdateAccountTokens(account.Id, newToken);
                                }
                                catch
                                {
                                    //Failed to refresh token
                                    //
                                    await _accountRepository.UpdateAccountTokens(account.Id, new Models.TwitchAuthResponseModel() { AccessToken = string.Empty, RefreshToken = string.Empty });
                                    await _userAuthentication.LogoutUser(account.Id);
                                }
                            }
                        }
                        catch { }
                    });
                }
                catch { }

                try
                {
                    await Task.Delay(1000 * 60 * 5, stoppingToken);
                }
                catch { }
            }
        }
    }
}
