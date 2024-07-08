namespace TuesdayMachines.Interfaces
{
    public interface IOnlinePlayersCounter
    {
        void AddPlayer(string game, string accountId, long time);
        int GetPlayerCount(string game);
        void Cleanup();
    }
}
