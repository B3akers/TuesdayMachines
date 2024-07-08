using System.Text.Json.Serialization;
using TuesdayMachines.WebSockets;

namespace TuesdayMachines.Interfaces
{
    public struct DuelDuelDuelSymbolReplacment
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("symbol")]
        public int Symbol { get; set; }
    };

    public struct DuelDuelDuelLineWin
    {
        [JsonPropertyName("line")]
        public int Line { get; set; }

        [JsonPropertyName("symbol")]
        public int Symbol { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("multi")]
        public int Multi { get; set; }

        [JsonPropertyName("win")]
        public long Win { get; set; }
    };


    [JsonDerivedType(typeof(DuelDuelDuelGameEventData))]
    [JsonDerivedType(typeof(DuelDuelGameSpinEventData))]

    public class DuelDuelDuelGameEventData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class DuelDuelGameSpinEventData : DuelDuelDuelGameEventData
    {
        [JsonPropertyName("stops")]
        public int[] Stops { get; set; }
        [JsonPropertyName("wins")]
        public List<DuelDuelDuelLineWin> Wins { get; set; }

        [JsonPropertyName("replacements")]
        public List<DuelDuelDuelSymbolReplacment> Replacements { get; set; }
    }

    public class DuelDuelDuelGameData
    {
        [JsonPropertyName("spins")]
        public List<DuelDuelDuelGameEventData> Spins { get; set; }
    }

    public enum DuelBaseGameSpinMode
    {
        Normal,
        ForceTrain,
        ForceDuel
    };

    public interface IDuelDuelDuelGame
    {
        DuelDuelDuelGameData SimulateGame(string clientSeed, string serverSeed, long nonce, long bet, DuelBaseGameSpinMode mode = DuelBaseGameSpinMode.Normal);
        string GetVersion();
    }
}
