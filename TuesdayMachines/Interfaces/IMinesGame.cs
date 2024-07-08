namespace TuesdayMachines.Interfaces
{
    public interface IMinesGame
    {
        int[] SimulateGame(string clientSeed, string serverSeed, long nonce, int mines);
        double GenerateMinesMulitpler(int diamonds, int mines);
        string GetVersion();
    }
}
