
using TuesdayMachines.Interfaces;

namespace TuesdayMachines.Services
{
    public class PlayerLiveCountUpdateService : BackgroundService
    {
        private readonly IOnlinePlayersCounter _onlinePlayersCounter;
        public PlayerLiveCountUpdateService(IOnlinePlayersCounter onlinePlayersCounter)
        {
            _onlinePlayersCounter = onlinePlayersCounter;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _onlinePlayersCounter.Cleanup();

                try
                {
                    await Task.Delay(1000 * 60);
                }
                catch { }
            }
        }
    }
}
