
using MongoDB.Driver;
using System.Net;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Services
{
    public class BroadcasterSubscribersList
    {
        public Dictionary<string, string> Subscribers = new();
        public long LastUpdate = 0;
    }

    public class WatchTimeUpdateService : BackgroundService
    {
        private readonly IBroadcastersRepository _broadcastersRepository;
        private readonly IAccountsRepository _accountRepository;
        private readonly IUserAuthentication _userAuthentication;
        private readonly IConfiguration _configuration;
        private readonly ITwitchApi _twitchApi;
        private readonly IPointsRepository _pointsRepository;

        private Dictionary<string, BroadcasterSubscribersList> _broadcastersSubscribers = new();

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
                    var list = await _broadcastersRepository.GetBroadcasters();
                    foreach (var broadcaster in list)
                    {
                        var account = await _accountRepository.GetAccountById(broadcaster.AccountId);

                        try
                        {
                            var accessToken = account.EncAccessToken.Decrypt(key);
                            if (string.IsNullOrEmpty(accessToken))
                                continue;

                            var userInfo = await _twitchApi.TwitchGetUserStremInfo(accessToken, account.TwitchId);
                            if (userInfo == null || userInfo.Type != "live")
                            {
                                _broadcastersSubscribers.Remove(account.TwitchId);
                                continue;
                            }

                            if (broadcaster.WatchPoints == 0 && broadcaster.WatchPointsSub == 0)
                                continue;

                            bool updatesubscribersList = false;
                            if (_broadcastersSubscribers.TryGetValue(account.TwitchId, out var subscribersList))
                            {
                                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - subscribersList.LastUpdate > 10800)
                                    updatesubscribersList = true;
                            }
                            else
                                updatesubscribersList = true;

                            if (updatesubscribersList)
                            {
                                subscribersList = new BroadcasterSubscribersList();

                                string subsCursor = string.Empty;
                                while (true)
                                {
                                    var subs = await _twitchApi.TwitchGetSubscriptions(accessToken, account.TwitchId, subsCursor);
                                    if (subs.Data.Length == 0)
                                        break;

                                    foreach (var sub in subs.Data)
                                        subscribersList.Subscribers[sub.UserId] = sub.Tier;

                                    if (subs.Pagination == null || string.IsNullOrEmpty(subs.Pagination.Cursor))
                                        break;

                                    subsCursor = subs.Pagination.Cursor;
                                }

                                subscribersList.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                _broadcastersSubscribers[account.TwitchId] = subscribersList;
                            }

                            string currentCursor = string.Empty;

                            List<PointModifyCommand> commands = new List<PointModifyCommand>(1000);

                            while (true)
                            {
                                var chatters = await _twitchApi.TwitchGetChatters(accessToken, account.TwitchId, currentCursor);
                                if (chatters.Data.Length == 0)
                                    break;

                                foreach (var chatter in chatters.Data)
                                {
                                    bool hasSub = subscribersList.Subscribers.ContainsKey(chatter.UserId);

                                    commands.Add(new PointModifyCommand() { TwitchUserId = chatter.UserId, value = hasSub ? broadcaster.WatchPointsSub : broadcaster.WatchPoints });
                                }

                                _pointsRepository.AddPoints(commands, account.Id);
                                commands.Clear();

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
                    }
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
