using System.Security.Cryptography;
using System.Text;

namespace TuesdayMachines.Utils
{
	public class HMACHashRandomGenerator
	{
		private string _seed;

		private HMACSHA256 hmacsha256;
		private byte[] _data;
		private byte[] _source;
		private int _index;
		private int _number;

		public HMACHashRandomGenerator(string clientSeed, string serverSeed, long nonce)
		{
			_seed = $"{clientSeed}:{nonce}:";
			_number = 0;
			_data = new byte[32];
			_source = new byte[_seed.Length * 2];

			hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(serverSeed));

			GenerateNewHash();
		}

		public int GetRandom(int min, int max)
		{
			int delta = max - min;
			if (delta == 0)
				return min;

			return min + (int)(GetUInt32() % delta);
		}

		private void GenerateNewHash()
		{
			int size = Encoding.UTF8.GetBytes($"{_seed}{_number}", _source);
			hmacsha256.TryComputeHash(new ReadOnlySpan<byte>(_source, 0, size), _data, out _);
			_index = 0;
			_number++;
		}

		private uint GetUInt32()
		{
			Span<byte> buffer = stackalloc byte[4];
			for (int i = 0; i < 4; i++)
				buffer[i] = _data[_index++];

			if (_index == 32)
				GenerateNewHash();

			return BitConverter.ToUInt32(buffer);
		}

		public void Dispose()
		{
			hmacsha256.Dispose();
		}
	};
}
