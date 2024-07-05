
using System.Text.Json;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;
using TuesdayMachines.WebSockets;

namespace TuesdayMachines.Services
{
    public class WinnerListPair
    {
        public List<RouletteGamePlayerWin> Winner = new();
        public List<PointModifyCommand> Command = new();
    };

    public class LiveRouletteBackgroundService : BackgroundService
    {
        private readonly IUserFairPlay _userFairPlay;
        private readonly LiveRouletteService _liveRouletteService;
        private readonly WebSocketRouletteHandler _handler;
        private readonly IPointsRepository _pointsRepository;
        public LiveRouletteBackgroundService(IUserFairPlay userFairPlay, LiveRouletteService liveRouletteService, WebSocketRouletteHandler handler, IPointsRepository pointsRepository)
        {
            _userFairPlay = userFairPlay;
            _liveRouletteService = liveRouletteService;
            _handler = handler;
            _pointsRepository = pointsRepository;
        }

        private bool IsRed(int number)
        {
            switch (number)
            {
                case 32:
                case 19:
                case 21:
                case 25:
                case 34:
                case 27:
                case 36:
                case 30:
                case 23:
                case 5:
                case 16:
                case 1:
                case 14:
                case 9:
                case 18:
                case 7:
                case 12:
                case 3:
                    return true;
                default:
                    return false;
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var betClosedPacket = JsonSerializer.Serialize(new RouletteGameBetClosed() { BetClosed = true });

            while (!stoppingToken.IsCancellationRequested)
            {
                _liveRouletteService.IsBetClosed = true;

                await _handler.SendToAll(betClosedPacket);

                var roundInfo = await _userFairPlay.GetNextLiveRouletteRoundInfo();

                int rouletteNumber = 0;

                Dictionary<string, WinnerListPair> winners = new Dictionary<string, WinnerListPair>();

                lock (_liveRouletteService.BetsLocker)
                {
                    var hash = $"{roundInfo.Client}:{roundInfo.Nonce}".HMAC(roundInfo.Server);

                    ulong number = 0;
                    for (var i = 0; i < hash.Length; i += 8)
                        number += BitConverter.ToUInt64(hash, i);

                    rouletteNumber = (int)(number % 37);

                    string numberString = rouletteNumber.ToString();

                    bool isZero = rouletteNumber == 0;
                    bool isRed = IsRed(rouletteNumber);
                    bool isBlack = !isZero && !isRed;

                    foreach (var betKeyValue in _liveRouletteService.PlayersBets)
                    {
                        long totalWin = 0;

                        var betValue = betKeyValue.Value;

                        foreach (var bet in betValue.Bets)
                        {
                            if (bet.Key == numberString)
                            {
                                totalWin += bet.Value * 36;
                            }
                            else if ((bet.Key == "black" && isBlack) || (bet.Key == "red" && isRed))
                            {
                                totalWin += bet.Value * 2;
                            }
                        }

                        if (totalWin > 0)
                        {
                            if (!winners.TryGetValue(betValue.WalletId, out var listWinners))
                            {
                                listWinners = new WinnerListPair();
                                winners.Add(betValue.WalletId, listWinners);
                            }

                            listWinners.Winner.Add(new RouletteGamePlayerWin() { Login = betValue.Login, TwitchId = betValue.TwitchId, Win = totalWin });
                            listWinners.Command.Add(new PointModifyCommand() { TwitchUserId = betValue.TwitchId, value = totalWin });
                        }
                    }

                    foreach (var win in winners)
                        _pointsRepository.AddPoints(win.Value.Command, win.Key);

                    _liveRouletteService.PlayersBets.Clear();

                    if (_liveRouletteService.LastsResults.Count >= 20)
                        _liveRouletteService.LastsResults.TryDequeue(out _);
                    _liveRouletteService.LastsResults.Enqueue(rouletteNumber);
                }

                _liveRouletteService.LastNonce = roundInfo.Nonce;

                foreach (var win in winners)
                {
                    RouletteGameWinnersResult result = new RouletteGameWinnersResult();
                    result.RoomClientsCount = _handler.GetRoomClientsCount(win.Key);
                    result.Winners = win.Value.Winner;

                    await _handler.SendToAllInRoom(JsonSerializer.Serialize(result), win.Key);
                }

                _liveRouletteService.NextCloseBetsTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 20;

                var finalResult = new RouletteGameResult()
                {
                    ClientsCount = _handler.GetClientsCount(),
                    NextSpinTime = _liveRouletteService.NextCloseBetsTime,
                    Nonce = roundInfo.Nonce,
                    Number = rouletteNumber
                };

                await _handler.SendToAll(JsonSerializer.Serialize(finalResult));

                _liveRouletteService.IsBetClosed = false;

                try
                {
                    await Task.Delay(1000 * 20, stoppingToken);
                }
                catch { }
            }
        }
    }
}
