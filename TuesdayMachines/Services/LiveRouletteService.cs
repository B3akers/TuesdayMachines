using System.Collections.Concurrent;
using System.Text.Json;
using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.WebSockets;

namespace TuesdayMachines.Services
{
    public class PlayerRoundBets
    {
        public long Balance;
        public string WalletId;
        public string TwitchId;
        public string Login;
        public ConcurrentDictionary<string, long> Bets = new ConcurrentDictionary<string, long>();
    };

    public class LiveRouletteService
    {
        public object BetsLocker = new object();
        public ConcurrentQueue<int> LastsResults = new ConcurrentQueue<int>();
        public long NextCloseBetsTime;
        public long LastNonce;

        public ConcurrentDictionary<string, PlayerRoundBets> PlayersBets = new ConcurrentDictionary<string, PlayerRoundBets>();

        private readonly IPointsRepository _pointsRepository;
        public LiveRouletteService(IPointsRepository pointsRepository)
        {
            _pointsRepository = pointsRepository;
        }

        private object _locker = new object();
        protected bool _isBetClosed;
        public bool IsBetClosed
        {
            get
            {
                lock (_locker)
                    return _isBetClosed;
            }
            set
            {
                lock (_locker)
                    _isBetClosed = value;
            }
        }

        public string GetValidBetNumber(string number)
        {
            if (number == "black"
                || number == "red")
                return number;

            if (int.TryParse(number, out var result))
                return result.ToString();

            return string.Empty;
        }

        public async Task<Tuple<string, string>> HandlePacket(WsConnectionInfo info, RouletteBasePacket packet)
        {
            if (packet is RoulettPlaceBetPacket)
            {
                var placeBet = (RoulettPlaceBetPacket)packet;
                var number = GetValidBetNumber(placeBet.Number);
                if (string.IsNullOrEmpty(number))
                    return null;

                if (IsBetClosed)
                {
                    await info.Send(JsonSerializer.Serialize(new RouletteGameError()
                    {
                        Error = "bets_closed"
                    }));
                    return null;
                }

                var isColorBet = number == "black" || number == "red";

                PointOperationResult takePointResult = default(PointOperationResult);

                lock (BetsLocker)
                {
                    if (!PlayersBets.TryGetValue(info.Account.Id, out var bets))
                    {
                        bets = new PlayerRoundBets() { WalletId = info.WalletId, TwitchId = info.Account.TwitchId, Login = info.Account.TwitchLogin };
                        PlayersBets.TryAdd(info.Account.Id, bets);
                    }

                    if (bets.WalletId != info.WalletId)
                        return null;

                    long currentValue = 0;
                    long totalBets = 0;
                    foreach (var bet in bets.Bets)
                    {
                        if (bet.Key == number)
                        {
                            if (isColorBet && bet.Value + placeBet.Amount > 30000)
                                return null;

                            if (!isColorBet && bet.Value + placeBet.Amount > 5000)
                                return null;

                            currentValue = bet.Value;
                        }

                        totalBets += bet.Value;
                    }

                    if (totalBets > 100000)
                        return null;

                    takePointResult = _pointsRepository.TakePoints(info.Account.TwitchId, info.WalletId, placeBet.Amount);
                    if (takePointResult.Success)
                    {
                        bets.Balance = takePointResult.Balance;
                        bets.Bets[number] = currentValue + placeBet.Amount;
                    }
                }

                if (!takePointResult.Success)
                {
                    await info.Send(JsonSerializer.Serialize(new RouletteGameError()
                    {
                        Error = "not_sufficient_funds"
                    }));
                    return null;
                }

                await info.Send(JsonSerializer.Serialize(new RouletteGameUpdateBalance()
                {
                    Balance = takePointResult.Balance
                }));

                return new Tuple<string, string>(JsonSerializer.Serialize(new RouletteGamePlaceBet()
                {
                    AccountId = info.Account.Id,
                    Amount = placeBet.Amount,
                    Number = number,
                    Login = info.Account.TwitchLogin
                }), info.WalletId);
            }

            return null;
        }
    }
}
