using System.Security.Cryptography;
using System.Text;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Utils;

namespace TuesdayMachines.Services
{
    public class MinesGameService : IMinesGame
    {
        private static readonly double[] _preCalculated = { Math.Pow(256.0, 1.0), Math.Pow(256.0, 2.0), Math.Pow(256.0, 3.0), Math.Pow(256.0, 4.0) };

        public string GetVersion()
        {
            return "v1";
        }

        public double GenerateMinesMulitpler(int diamonds, int mines)
        {
            var n = 25;
            var x = 25 - mines;
            var first = Combination((double)n, (double)diamonds);
            var second = Combination((double)x, (double)diamonds);

            var result = 0.99 * (first / second);
            result = Math.Round(result * 100) / 100;

            return result;
        }

        public int[] SimulateGame(string clientSeed, string serverSeed, long nonce, int mines)
        {
            List<int> numbers = new List<int>(25);
            for (var i = 0; i < 25; i++)
                numbers.Add(i);

            int[] values = new int[mines];
            Span<byte> resultSha = stackalloc byte[32];

            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(serverSeed)))
            {
                int number = 0;
                while (mines > 0)
                {
                    hmacsha256.TryComputeHash(Encoding.UTF8.GetBytes($"{clientSeed}:{nonce}:{number}"), resultSha, out _);

                    for (var i = 0; i < 32; i += 4)
                    {
                        if (mines == 0)
                            break;

                        double value_1 = (resultSha[i] / _preCalculated[0]);
                        double value_2 = (resultSha[i + 1] / _preCalculated[1]);
                        double value_3 = (resultSha[i + 2] / _preCalculated[2]);
                        double value_4 = (resultSha[i + 3] / _preCalculated[3]);

                        double final_value = (value_1 + value_2 + value_3 + value_4) * numbers.Count;

                        var index = (int)final_value;

                        values[values.Length - mines] = numbers[index];
                        numbers.RemoveAt(index);

                        mines--;
                    }

                    number++;
                }
            }

            return values;
        }

        private double Factorial(double number)
        {
            var value = number;
            for (var i = number; i > 1; i--)
                value *= i - 1;
            return value;
        }

        private double Combination(double x, double d)
        {
            if (x == d) return 1;
            return Factorial(x) / (Factorial(d) * Factorial(x - d));
        }
    }
}
