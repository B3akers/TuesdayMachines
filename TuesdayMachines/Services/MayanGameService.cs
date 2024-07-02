using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuesdayMachines.Services
{
    public class MayanReelData
    {
        [JsonPropertyName("baseGame")]
        public int[][] BaseGame { get; set; }

        [JsonPropertyName("respinA")]
        public int[][] RespinA { get; set; }

        [JsonPropertyName("respinB")]
        public int[][] RespinB { get; set; }

        [JsonPropertyName("respinC")]
        public int[][] RespinC { get; set; }

        public int[][] GetReelByIndex(int index)
        {
            switch (index)
            {
                case 0: return BaseGame;
                case 1: return RespinA;
                case 2: return RespinB;
                case 3: return RespinB;
                default: return RespinC;
            }
        }
    };

    public class MayanGameHashRandom
    {
        private string _clientSeed;
        private string _serverSeed;
        private long _nonce;
        private int _number;

        private byte[] _data;
        private int _index;

        public MayanGameHashRandom(string clientSeed, string serverSeed, long nonce)
        {
            _clientSeed = clientSeed;
            _serverSeed = serverSeed;
            _nonce = nonce;
            _number = 0;
        }

        public int GetRandom(int min, int max)
        {
            int delta = max - min;
            if (delta == 0)
                return min;

            uint currentValue = BitConverter.ToUInt32([GetNextByteValue(), GetNextByteValue(), GetNextByteValue(), GetNextByteValue()]);

            return min + (int)(currentValue % delta);
        }

        private void GenerateNewHash()
        {
            _data = $"{_clientSeed}:{_nonce}:{_number}".HMAC(_serverSeed);
            _index = 0;
            _number++;
        }

        private byte GetNextByteValue()
        {
            if (_data == null || _index >= _data.Length)
                GenerateNewHash();

            return _data[_index++];
        }
    };

    public class MayanGameService : IMayanGame
    {
        private MayanReelData _reelData;
        private long[] _payoutTable;

        public MayanGameService()
        {
            _reelData = JsonSerializer.Deserialize<MayanReelData>(Properties.Resources._mayanReelData);
            _payoutTable = new long[]
             {
                  4, 6, 8,
                  4, 6, 8,
                  4, 6, 8,
                  4, 6, 8,
                  25, 50, 150,
                  15, 30, 75,
                  10, 25, 50,
                  10, 25, 50
             };
        }

        public string GetVersion()
        {
            return "v1";
        }

        public MayanGameData SimulateGame(string clientSeed, string serverSeed, long nonce, long bet)
        {
            long betMultiplayer = bet / 25;

            var result = new MayanGameData();
            result.Spins = new List<MayanGameSpinData>();

            MayanGameHashRandom random = new MayanGameHashRandom(clientSeed, serverSeed, nonce);

            int currentSpin = 0;
            long totalPayout = 0;

            Span<int> stops = stackalloc int[5];
            Span<int> replacements = stackalloc int[6];

            Span<int> lockedSymbols = stackalloc int[5 * 3];
            for (var i = 0; i < 5 * 3; i++)
                lockedSymbols[i] = -1;

            Span<int> possibleLockedSymbols = stackalloc int[5 * 3];

            Span<int> payLines = stackalloc int[8 * 5];

            List<int> lockedSymbolsThisRound = new List<int>(8);
            List<int> foundSymbols = new List<int>(8);

            int firstLockedSymbol = -1;
            int secondLockedSymbol = -1;

            do
            {
                var spinResult = new MayanGameSpinData();

                long currentPayout = 0;

                var reels = _reelData.GetReelByIndex(currentSpin);

                //Get stops
                //
                for (var i = 0; i < stops.Length; i++)
                    stops[i] = random.GetRandom(0, reels[i].Length);

                //Get replacments
                //
                if (currentSpin > 0)
                {
                    List<int> availableReplacments = new List<int>(8) { 0, 1, 2, 3, 4, 5, 6, 7 };

                    if (random.GetRandom(0, 1000) > 625)
                    {
                        if (firstLockedSymbol != -1)
                            availableReplacments.Remove(firstLockedSymbol);

                        if (secondLockedSymbol != -1)
                            availableReplacments.Remove(secondLockedSymbol);
                    }

                    for (var i = 0; i < replacements.Length; i++)
                    {
                        var index = random.GetRandom(0, availableReplacments.Count);
                        replacements[i] = availableReplacments[index];
                        availableReplacments.RemoveAt(index);
                    }
                }
                else
                {
                    replacements = [4, 5, 6, 7, 1, 0];
                }

                lockedSymbolsThisRound.Clear();
                for (var i = 0; i < 5 * 3; i++)
                    possibleLockedSymbols[i] = lockedSymbols[i];

                //Get board info
                //
                for (var i = 0; i < 5; i++)
                {
                    var stopIndex = stops[i];
                    var reel = reels[i];

                    for (var j = 0; j < 3; j++)
                    {
                        var symbolIndex = i * 3 + j;

                        //Check if gem is active, last should be active only in 2,3th spin
                        //
                        if ((i == 0 || i == 4) && (3 - j) > currentSpin)
                            continue;

                        //Position is already locked in past spins
                        //
                        if (lockedSymbols[symbolIndex] != -1)
                            continue;

                        var gemIndex = stopIndex + j;
                        if (gemIndex >= reel.Length)
                            gemIndex = gemIndex - reel.Length;

                        var gemValue = reel[gemIndex];
                        if (gemValue < 0)
                            gemValue = replacements[gemValue + replacements.Length];

                        //Check if we can add this to current locked symbols
                        //

                        for (var y = -1; y < 2; y++)
                        {
                            //It's on edge skip non existing reel
                            //
                            if (i + y < 0 || i + y >= 5)
                                continue;

                            var start = (i + y) * 3;
                            for (var k = 0; k < 3; k++)
                            {
                                if (lockedSymbols[start + k] == gemValue)
                                {
                                    lockedSymbols[symbolIndex] = gemValue;
                                    break;
                                }
                            }

                            //Break if symbol is locked
                            //
                            if (lockedSymbols[symbolIndex] != -1)
                                break;
                        }

                        //Add to possible list
                        //
                        possibleLockedSymbols[symbolIndex] = gemValue;

                        //Symbol is locked add to locked this round list
                        //
                        if (lockedSymbols[symbolIndex] != -1)
                        {
                            lockedSymbolsThisRound.Add(symbolIndex);
                            continue;
                        }
                    }
                }

                for (var i = 0; i < 5; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var symbolIndex = i * 3 + j;
                        var symbol = possibleLockedSymbols[symbolIndex];

                        //Dont check already locked sybmols or non existing
                        //
                        if (lockedSymbols[symbolIndex] == symbol)
                            continue;

                        int reelsConnect = 0;
                        foundSymbols.Clear();

                        //Right
                        //
                        for (var y = i + 1; y < 5; y++)
                        {
                            bool found = false;

                            for (var k = 0; k < 3; k++)
                            {
                                var index = y * 3 + k;
                                if (possibleLockedSymbols[index] == symbol)
                                {
                                    foundSymbols.Add(index);
                                    found = true;
                                }
                            }

                            if (!found)
                                break;

                            reelsConnect++;
                        }

                        //Left
                        //
                        for (var y = i - 1; y >= 0; y--)
                        {
                            bool found = false;

                            for (var k = 0; k < 3; k++)
                            {
                                var index = y * 3 + k;
                                if (possibleLockedSymbols[index] == symbol)
                                {
                                    foundSymbols.Add(index);
                                    found = true;
                                }
                            }

                            if (!found)
                                break;

                            reelsConnect++;
                        }

                        if (reelsConnect < 2)
                        {
                            possibleLockedSymbols[symbolIndex] = -1;
                            foreach (var index in foundSymbols)
                                possibleLockedSymbols[index] = -1;
                        }
                    }
                }


                //Detect new locked symbols
                //
                for (var i = 0; i < 5; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var symbolIndex = i * 3 + j;
                        var symbol = possibleLockedSymbols[symbolIndex];
                        if (symbol == -1)
                            continue;

                        if (symbol == lockedSymbols[symbolIndex])
                            continue;

                        if (firstLockedSymbol == -1)
                            firstLockedSymbol = symbol;
                        else if (secondLockedSymbol == -1 && symbol != firstLockedSymbol)
                            secondLockedSymbol = symbol;

                        lockedSymbols[symbolIndex] = symbol;
                        lockedSymbolsThisRound.Add(symbolIndex);
                    }
                }

                //Calculate current win
                //

                for (var i = 0; i < 8 * 5; i++)
                    payLines[i] = 0;

                for (var i = 0; i < 5; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var symbolIndex = i * 3 + j;
                        var symbol = lockedSymbols[symbolIndex];
                        if (symbol == -1)
                            continue;

                        payLines[(symbol * 5) + i]++;
                    }
                }

                for (var i = 0; i < 8; i++)
                {
                    var lineCount = 0;
                    var stackWays = 1;

                    for (var j = 0; j < 5; j++)
                    {
                        var count = payLines[i * 5 + j];
                        if (count == 0)
                            continue;

                        stackWays *= count;
                        lineCount++;
                    }

                    if (lineCount >= 3)
                    {
                        currentPayout += _payoutTable[(i * 3) + (lineCount - 3)] * stackWays;
                    }
                }

                int currentMulti = 1;
                //if (currentSpin >= 3)
                //    currentMulti = 3;
                //else if (currentSpin >= 2)
                //    currentMulti = 2;
                //
                //if (lockedSymbolsThisRound.Count == 0
                //    && currentSpin >= 3
                //    && random.GetRandom(0, 100) > 90)
                //{
                //    currentMulti = random.GetRandom(4, 15);
                //}

                currentPayout *= betMultiplayer;
                currentPayout *= currentMulti;
                totalPayout += currentPayout;

                spinResult.Multi = currentMulti;
                spinResult.Replacements = replacements.ToArray();
                spinResult.Stops = stops.ToArray();
                spinResult.NewLockedSymbols = lockedSymbolsThisRound.ToArray();
                spinResult.LockedSymbols = lockedSymbols.ToArray();
                spinResult.SpinWin = currentPayout;

                result.Spins.Add(spinResult);

                currentSpin++;
            } while (lockedSymbolsThisRound.Count > 0);

            result.Bet = bet;
            result.TotalWin = totalPayout;

            return result;
        }
    }
}
