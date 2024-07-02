using System.Text.Json.Serialization;

namespace TuesdayMachines.Interfaces
{
    public class MayanGameSpinData
    {
        [JsonPropertyName("spinWin")]
        public long SpinWin { get; set; }

        [JsonPropertyName("multi")]
        public int Multi { get; set; }

        [JsonPropertyName("stops")]
        public int[] Stops { get; set; }

        [JsonPropertyName("replacements")]
        public int[] Replacements { get; set; }

        [JsonPropertyName("lockedSymbols")]
        public int[] LockedSymbols { get; set; }

        [JsonPropertyName("newLockedSymbols")]
        public int[] NewLockedSymbols { get; set; }
    };

    public class MayanGameData
    {
        [JsonPropertyName("totalWin")]
        public long TotalWin { get; set; }

        [JsonPropertyName("bet")]
        public long Bet { get; set; }

        [JsonPropertyName("currentBalance")]
        public long CurrentBalance { get; set; }

        [JsonPropertyName("spins")]
        public List<MayanGameSpinData> Spins { get; set; }
    };

    public interface IMayanGame
    {
        MayanGameData SimulateGame(string clientSeed, string serverSeed, long nonce, long bet);
        string GetVersion();
    }
}
