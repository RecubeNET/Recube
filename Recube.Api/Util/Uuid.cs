using System;
using System.Security.Cryptography;

namespace Recube.Api.Util
{
	public class Uuid
	{
		private static readonly long SerialVersionUid = -4856846361193249489L;
		public readonly long LeastSigBits;

		public readonly long MostSigBits;

		private Uuid(byte[] data)
		{
			long msb = 0;
			long lsb = 0;
			if (data.Length != 16)
				throw new IndexOutOfRangeException("data must be 16 bytes in length");
			for (var i = 0; i < 8; i++)
				msb = (msb << 8) | (data[i] & 0xff);
			for (var i = 8; i < 16; i++)
				lsb = (lsb << 8) | (data[i] & 0xff);
			MostSigBits = msb;
			LeastSigBits = lsb;
		}

		public Uuid(long mostSigBits, long leastSigBits)
		{
			MostSigBits = mostSigBits;
			LeastSigBits = leastSigBits;
		}

		public static Uuid RandomUuid()
		{
			var randomBytes = new byte[16];
			new Random().NextBytes(randomBytes);
			randomBytes[6] &= 0x0f; /* clear version        */
			randomBytes[6] |= 0x40; /* set to version 4     */
			randomBytes[8] &= 0x3f; /* clear variant        */
			randomBytes[8] |= 0x80; /* set to IETF variant  */
			return new Uuid(randomBytes);
		}

		public static Uuid NameUuidFromBytes(byte[] name)
		{
			var md5Bytes = MD5.Create().ComputeHash(name);
			md5Bytes[6] &= 0x0f; /* clear version        */
			md5Bytes[6] |= 0x30; /* set to version 3     */
			md5Bytes[8] &= 0x3f; /* clear variant        */
			md5Bytes[8] |= 0x80; /* set to IETF variant  */
			return new Uuid(md5Bytes);
		}

		public static Uuid FromString(string name)
		{
			var components = name.Split("-");
			if (components.Length != 5)
				throw new ArgumentException("Invalid UUID string: " + name);
			for (var i = 0; i < 5; i++)
				components[i] = "0x" + components[i];

			var mostSigBits = long.Parse(components[0]);
			mostSigBits <<= 16;
			mostSigBits |= long.Parse(components[1]);
			mostSigBits <<= 16;
			mostSigBits |= long.Parse(components[2]);

			var leastSigBits = long.Parse(components[3]);
			;
			leastSigBits <<= 48;
			leastSigBits |= long.Parse(components[4]);

			return new Uuid(mostSigBits, leastSigBits);
		}

		public int Version()
		{
			// Version is bits masked by 0x000000000000F000 in MS long
			return (int) ((MostSigBits >> 12) & 0x0f);
		}

		private static long TripleShift(long n, int s)
		{
			if (n >= 0)
				return n >> s;
			return (n >> s) + (2 << ~s);
		}


		public int Variant()
		{
			// This field is composed of a varying number of bits.
			// 0    -    -    Reserved for NCS backward compatibility
			// 1    0    -    The IETF aka Leach-Salz variant (used by this class)
			// 1    1    0    Reserved, Microsoft backward compatibility
			// 1    1    1    Reserved for future definition.
			return (int) (TripleShift(LeastSigBits, (int) (64 - TripleShift(LeastSigBits, 62))) & (LeastSigBits >> 63));
		}

		public long Timestamp()
		{
			if (Version() != 1)
			{
				throw new NotSupportedException("Not a time-based UUID");
			}

			return (MostSigBits & 0x0FFFL) << 48
			       | ((MostSigBits >> 16) & 0x0FFFFL) << 32
			       | TripleShift(MostSigBits, 32);
		}

		public int ClockSequence()
		{
			if (Version() != 1)
			{
				throw new NotSupportedException("Not a time-based UUID");
			}

			return (int) TripleShift(LeastSigBits & 0x3FFF000000000000L, 48);
		}

		public long Node()
		{
			if (Version() != 1)
			{
				throw new NotSupportedException("Not a time-based UUID");
			}

			return LeastSigBits & 0x0000FFFFFFFFFFFFL;
		}

		public override string ToString()
		{
			return Digits(MostSigBits >> 32, 8) + "-" +
			       Digits(MostSigBits >> 16, 4) + "-" +
			       Digits(MostSigBits, 4) + "-" +
			       Digits(LeastSigBits >> 48, 4) + "-" +
			       Digits(LeastSigBits, 12);
		}

		private static string Digits(long val, int digits)
		{
			var hi = 1L << (digits * 4);
			return (hi | (val & (hi - 1))).ToString("X").Substring(1);
		}

		public override int GetHashCode()
		{
			var hilo = MostSigBits ^ LeastSigBits;
			return (int) (hilo >> 32) ^ (int) hilo;
		}

		public override bool Equals(object obj)
		{
			if (null == obj || !(obj is Uuid))
				return false;
			var id = (Uuid) obj;
			return MostSigBits == id.MostSigBits &&
			       LeastSigBits == id.LeastSigBits;
		}

		public int CompareTo(Uuid val)
		{
			// The ordering is intentionally set up so that the UUIDs
			// can simply be numerically compared as two numbers
			return MostSigBits < val.MostSigBits ? -1 :
				MostSigBits > val.MostSigBits ? 1 :
				LeastSigBits < val.LeastSigBits ? -1 :
				LeastSigBits > val.LeastSigBits ? 1 :
				0;
		}
	}
}