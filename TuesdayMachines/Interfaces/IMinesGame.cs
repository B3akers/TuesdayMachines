namespace TuesdayMachines.Interfaces
{
    public class MinesGameData
    {
        public int[] Bombs { get; set; }
    };


    public interface IMinesGame
    {
        MinesGameData SimulateGame(string clientSeed, string serverSeed, long nonce, int mines);
        double GenerateMinesMulitpler(int diamonds, int mines);
        string GetVersion();
    }
}
