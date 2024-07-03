using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Services
{
    public class PlinkoGameService : IPlinkoGame
    {
        private readonly double[] _preCalculated = { Math.Pow(256.0, 1.0), Math.Pow(256.0, 2.0), Math.Pow(256.0, 3.0), Math.Pow(256.0, 4.0) };
        private readonly double[] _plinkoHighMultiply =
        {
            1000.0,
            130.0,
            26.0,
            9.0,
            4.0,
            2.0,
            0.2,
            0.2,
            0.2,
            0.2,
            0.2,
            2.0,
            4.0,
            9.0,
            26.0,
            130.0,
            1000.0
        };

        public string GetVersion()
        {
            return "v1";
        }

        public PlinkoGameData SimulateGame(string clientSeed, string serverSeed, long nonce, long bet)
        {
            PlinkoGameData result = new PlinkoGameData();

            int index = 0;
            int total = 16;
            int number = 0;

            int[] path = new int[16];

            while (total > 0)
            {
                var resultSha = $"{clientSeed}:{nonce}:{number}".HMAC(serverSeed);

                for (var i = 0; i < 32; i += 4)
                {
                    double value_1 = (resultSha[i] / _preCalculated[0]);
                    double value_2 = (resultSha[i + 1] / _preCalculated[1]);
                    double value_3 = (resultSha[i + 2] / _preCalculated[2]);
                    double value_4 = (resultSha[i + 3] / _preCalculated[3]);

                    double final_value = (value_1 + value_2 + value_3 + value_4) * 2.0;

                    bool direction = final_value >= 1.0;
                    if (direction)
                        index++;

                    path[16 - total] = (direction ? 1 : 0);

                    total--;
                }

                number++;
            }

            result.Bet = bet;
            result.Path = path;
            result.TotalWin = (long)(_plinkoHighMultiply[index] * result.Bet);

            return result;
        }
    }
}
