namespace TuesdayMachines.Interfaces
{
    public class PlinkoGameData
    {
        public long TotalWin { get; set; }
        public long Bet { get; set; }
        public long CurrentBalance { get; set; }
        public int[] Path { get; set; }
    };

    public interface IPlinkoGame
    {
        PlinkoGameData SimulateGame(string clientSeed, string serverSeed, long nonce, long bet);
        string GetVersion();
    }
}
