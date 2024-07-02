using MongoDB.Driver;
namespace TuesdayMachines.Services
{
    public class DatabaseCleanerService : BackgroundService
    {
        private readonly DatabaseService _databaseService;
        public DatabaseCleanerService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                {
                    var spins = _databaseService.GetSpins();
                    var time = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeSeconds();
                    await spins.DeleteManyAsync(x => x.Datetime < time);
                }

                {
                    var devices = _databaseService.GetDevices();
                    var time = DateTimeOffset.UtcNow.AddMonths(-2).ToUnixTimeSeconds();
                    await devices.DeleteManyAsync(x => x.LastUse < time);
                }

                {
                    var spins = _databaseService.GetSpinsStat();
                    var time = DateTimeOffset.UtcNow.AddMonths(-1).ToUnixTimeSeconds();
                    await spins.DeleteManyAsync(x => x.Datetime < time);
                }

                try
                {
                    await Task.Delay(3600 * 1000 * 24);
                }
                catch { }
            }
        }
    }
}
