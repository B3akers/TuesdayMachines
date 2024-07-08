using System.Collections.Concurrent;
using TuesdayMachines.Interfaces;
using TuesdayMachines.WebSockets;

namespace TuesdayMachines.Services
{
    public class OnlinePlayersCounterService : IOnlinePlayersCounter
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, long>> gamesOnlinePlayers = new();
        private readonly WebSocketRouletteHandler _rouletteHandler;
        public OnlinePlayersCounterService(WebSocketRouletteHandler rouletteHandler)
        {
            _rouletteHandler = rouletteHandler;
        }

        public void AddPlayer(string game, string accountId, long time)
        {
            if (!gamesOnlinePlayers.TryGetValue(game, out var gamePlayers))
                gamePlayers = gamesOnlinePlayers.GetOrAdd(game, new ConcurrentDictionary<string, long>());

            gamePlayers.AddOrUpdate(accountId, time, (key, oldValue) => time);
        }

        public int GetPlayerCount(string game)
        {
            if (game == "roulette")
                return _rouletteHandler.GetClientsCount();

            if (gamesOnlinePlayers.TryGetValue(game, out var gamePlayers))
                return gamePlayers.Count;

            return 0;
        }

        public void Cleanup()
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            List<string> keysToRemove = new List<string>();
            foreach (var game in gamesOnlinePlayers)
            {
                keysToRemove.Clear();

                foreach (var player in game.Value)
                {
                    if (currentTime - player.Value > (60 * 5))
                    {
                        keysToRemove.Add(player.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    game.Value.TryRemove(key, out _);
                }
            }
        }
    }
}
