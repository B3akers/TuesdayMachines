using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Services
{
    //WILD - 0
    //10 - 1
    //J - 2
    //Q - 3
    //K - 4
    //A - 5
    //BULL = 6
    //BANDIT = 7
    //SKULL = 8
    //WHISKY = 9
    //REVOLVER = 10
    //DUEL = 11
    //DUEL_BONUS = 12
    //NORMAL_BONUS = 13

    public struct DuelBaseSpinResult
    {
        public DuelDuelGameSpinEventData Event;
        public DuelBaseGameSpinMode BonusResult;
    }

    public class DuelDuelDuelReelData
    {
        [JsonPropertyName("baseGame")]
        public int[][] BaseGame { get; set; }

        [JsonPropertyName("trainBonus")]
        public int[][] TrainBonus { get; set; }

        [JsonPropertyName("duelBonus")]
        public int[][] DuelBonus { get; set; }
    };

    public class DuelDuelDuelGameService : IDuelDuelDuelGame
    {
        private readonly DuelDuelDuelReelData _reelsData;
        private readonly int[][] _lines = new int[15][];
        private readonly long[] _payoutTable;

        public DuelDuelDuelGameService()
        {
            _reelsData = JsonSerializer.Deserialize<DuelDuelDuelReelData>(Properties.Resources._x3duelReelData);

            _payoutTable = new long[]
             {
                  400, 400, 400,
                  2, 10, 20,
                  2, 10, 20,
                  2, 10, 20,
                  2, 10, 20,
                  2, 10, 20,
                  10, 50, 100,
                  10, 50, 100,
                  20, 100, 200,
                  20, 100, 200,
                  40, 200, 400,
             };

            _lines[0] = [0, 5, 10, 15, 20];
            _lines[1] = [1, 6, 11, 16, 21];
            _lines[2] = [2, 7, 12, 17, 22];
            _lines[3] = [3, 8, 13, 18, 23];
            _lines[4] = [4, 9, 14, 19, 24];
            _lines[5] = [0, 6, 12, 18, 24];
            _lines[6] = [4, 8, 12, 16, 20];
            _lines[7] = [0, 6, 10, 16, 20];
            _lines[8] = [1, 5, 11, 15, 21];
            _lines[9] = [1, 7, 11, 17, 21];
            _lines[10] = [2, 6, 12, 16, 22];
            _lines[11] = [2, 8, 12, 18, 22];
            _lines[12] = [3, 7, 13, 17, 23];
            _lines[13] = [3, 9, 13, 19, 23];
            _lines[14] = [4, 8, 14, 18, 24];
        }

        public string GetVersion()
        {
            return "v1";
        }

        private void GenerateStopsAndFillBoard(HMACHashRandomGenerator random, int[] stops, int[][] reelData, Span<int> board, Span<int> wildsMultiplayer)
        {
            for (int i = 0; i < stops.Length; i++)
                stops[i] = random.GetRandom(0, reelData[i].Length);

            for (int i = 0; i < stops.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var index = stops[i] + j;
                    if (index >= reelData[i].Length)
                        index = index - reelData[i].Length;

                    board[i * 5 + j] = reelData[i][index];
                    wildsMultiplayer[i * 5 + j] = 0;
                }
            }
        }

        private DuelDuelDuelSymbolReplacment GenerateReplacement(HMACHashRandomGenerator random, List<int> symbolsFreePlaces, int symbol)
        {
            var index = random.GetRandom(0, symbolsFreePlaces.Count);
            var indexOnBoard = symbolsFreePlaces[index];
            symbolsFreePlaces.RemoveAt(index);

            return new DuelDuelDuelSymbolReplacment()
            {
                Index = indexOnBoard,
                Symbol = symbol,
            };
        }

        private List<DuelDuelDuelLineWin> GenerateWins(ReadOnlySpan<int> gameBoard, ReadOnlySpan<int> wildsMultiplayer, long bet)
        {
            List<DuelDuelDuelLineWin> wins = new List<DuelDuelDuelLineWin>();

            var betMulti = bet / 20;

            for (int i = 0; i < _lines.Length; i++)
            {
                var line = _lines[i];
                int symbol = gameBoard[line[0]];
                int multi = wildsMultiplayer[line[0]];
                int lineSize = 1;

                if (symbol < 11)
                {
                    for (int j = 1; j < line.Length; j++)
                    {
                        var index = line[j];
                        var nextSymbol = gameBoard[index];

                        if (nextSymbol == symbol
                            || nextSymbol == 0
                            || symbol == 0)
                        {
                            multi += wildsMultiplayer[index];

                            if (symbol == 0 && nextSymbol != 0)
                            {
                                symbol = nextSymbol;
                            }

                            lineSize++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (multi == 0)
                    multi = 1;

                if (lineSize >= 3)
                {
                    var payout = betMulti * _payoutTable[symbol * 3 + (lineSize - 3)];

                    wins.Add(new DuelDuelDuelLineWin()
                    {
                        Line = i,
                        Symbol = symbol,
                        Count = lineSize,
                        Multi = multi,
                        Win = payout * multi
                    });
                }
            }

            return wins;
        }

        private DuelBaseSpinResult GetBaseGameSpin(HMACHashRandomGenerator random, DuelBaseGameSpinMode mode, long bet)
        {
            DuelDuelGameSpinEventData spin = new DuelDuelGameSpinEventData();
            spin.Id = "base_spin";
            spin.Stops = new int[5];
            spin.Replacements = new List<DuelDuelDuelSymbolReplacment>();

            var baseGame = _reelsData.BaseGame;

            Span<int> wildsMultiplayer = stackalloc int[5 * 5];
            Span<int> gameBoard = stackalloc int[5 * 5];
            GenerateStopsAndFillBoard(random, spin.Stops, baseGame, gameBoard, wildsMultiplayer);

            int trainSymbolsCount = 0;
            int duelSymbolsCount = 0;

            List<int> symbolsFreePlaces = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24];

            if (mode == DuelBaseGameSpinMode.Normal)
            {
                for (var i = 0; i < 3; i++)
                    if (random.GetRandom(0, 1000) < 125)
                        trainSymbolsCount++;

                if (trainSymbolsCount != 3)
                    for (var i = 0; i < 3; i++)
                        if (random.GetRandom(0, 1000) < 100)
                            duelSymbolsCount++;

                if (trainSymbolsCount == 0 && duelSymbolsCount == 0)
                {
                    //VS logic
                }
                else if (trainSymbolsCount == 3)
                {
                    mode = DuelBaseGameSpinMode.ForceTrain;
                }
                else if (duelSymbolsCount == 3)
                {
                    mode = DuelBaseGameSpinMode.ForceDuel;
                }
            }
            else if (mode == DuelBaseGameSpinMode.ForceTrain)
            {
                trainSymbolsCount = 3;
            }
            else if (mode == DuelBaseGameSpinMode.ForceDuel)
            {
                duelSymbolsCount = 3;
            }

            for (var i = 0; i < trainSymbolsCount; i++)
            {
                var replacment = GenerateReplacement(random, symbolsFreePlaces, 13);
                spin.Replacements.Add(replacment);
                gameBoard[replacment.Index] = 13;
            }

            for (var i = 0; i < duelSymbolsCount; i++)
            {
                var replacment = GenerateReplacement(random, symbolsFreePlaces, 12);
                spin.Replacements.Add(replacment);
                gameBoard[replacment.Index] = 12;
            }

            spin.Wins = GenerateWins(gameBoard, wildsMultiplayer, bet);

            return new DuelBaseSpinResult
            {
                Event = spin,
                BonusResult = mode
            };
        }

        public DuelDuelDuelGameData SimulateGame(string clientSeed, string serverSeed, long nonce, long bet, DuelBaseGameSpinMode mode)
        {
            HMACHashRandomGenerator random = new HMACHashRandomGenerator(clientSeed, serverSeed, nonce);

            var baseSpinResult = GetBaseGameSpin(random, mode, bet);

            DuelDuelDuelGameData result = new DuelDuelDuelGameData();
            result.Spins = new List<DuelDuelDuelGameEventData>();
            result.Spins.Add(baseSpinResult.Event);

            return result;
        }
    }
}
