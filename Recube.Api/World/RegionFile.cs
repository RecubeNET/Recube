using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Recube.Api.World
{
	public class RegionFile
	{
		private static readonly int VERSION_GZIP = 1;
		private static readonly int VERSION_DEFLATE = 2;
		private static readonly int SECTOR_BYTES = 4096;
		private static readonly int SECTOR_INTS = SECTOR_BYTES / 4;
		private static readonly int CHUNK_HEADER_SIZE = 5;

		private static readonly byte[] emptySector = new byte[4096];
		private readonly int[] chunkTimestamps;
		private readonly FileStream file;

		private readonly string fileName;
		private readonly int[] offsets;
		private readonly List<bool> sectorFree;
		private int sizeDelta;

		public RegionFile(string fileName)
		{
			offsets = new int[SECTOR_INTS];
			chunkTimestamps = new int[SECTOR_INTS];

			this.fileName = fileName;

			Console.WriteLine("REGION LOAD: " + fileName);
			//TODO: DOnt Hardcore World
			Directory.CreateDirectory("./World/region");
			sizeDelta = 0;
			try
			{
				if (File.Exists(fileName))
				{
					LastModified = File.GetLastWriteTime(fileName).Ticks;
				}

				file = File.Open(this.fileName, FileMode.OpenOrCreate);
				if (file.Length < SECTOR_BYTES)
				{
					file.SetLength(SECTOR_BYTES * 2);
					sizeDelta += SECTOR_BYTES * 2;
				}

				if ((file.Length & 0xfff) != 0)
				{
					//File SIze is not a multiple of 4KB, grow it
					for (var i = 0; i < (file.Length & 0xfff); ++i)
					{
						file.Write(BitConverter.GetBytes(0));
					}
				}

				var nSectors = (int) file.Length / SECTOR_BYTES;
				sectorFree = new List<bool>(nSectors);

				for (var i = 0; i < nSectors; ++i)
				{
					sectorFree.Add(true);
				}

				sectorFree[0] = false;
				sectorFree[1] = false;

				file.Seek(0, SeekOrigin.Begin);
				for (var i = 0; i < SECTOR_INTS; ++i)
				{
					var offSetByte = new byte[4];
					file.Read(offSetByte, 0, 4);
					var offset = BitConverter.ToInt32(offSetByte);
					offsets[i] = offset;
					if (offset != 0 && (offset >> 8) + (offset & 0xFF) <= sectorFree.Count)
					{
						for (var sectorSum = 0; sectorSum < (offset & 0xFF); ++sectorSum)
						{
							sectorFree[(offset >> 8) + sectorSum] = false;
						}
					}
				}

				for (var i = 0; i < SECTOR_INTS; ++i)
				{
					var offSetByte = new byte[4];
					file.Read(offSetByte, 0, 4);
					var lastModValue = BitConverter.ToInt32(offSetByte);
					chunkTimestamps[i] = lastModValue;
				}

				file.Flush();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		public long LastModified { get; }

		public BinaryReader? getChunkDataInputStream(int x, int z)
		{
			if (outOfBounds(x, z))
			{
				Console.WriteLine($"READ {x}, {z} out of bounds");
				return null;
			}

			try
			{
				var offset = getOffset(x, z);
				if (offset == 0)
				{
					Console.WriteLine($"READ {x}, {z} Miss");
					return null;
				}

				var sectorNumber = offset >> 8;
				var numSectors = offset & 0xFF;

				if (sectorNumber + numSectors > sectorFree.Count)
				{
					Console.WriteLine($"READ {x}, {z} Invalid Sector");
					return null;
				}

				file.Seek(sectorNumber * SECTOR_BYTES, SeekOrigin.Begin);
				var length = file.ReadByte();

				if (length > SECTOR_BYTES * numSectors)
				{
					Console.WriteLine($"READ {x}, {z} Invalid length: {length} > 4096 * {numSectors}");
					return null;
				}

				var version = (byte) file.ReadByte();
				if (version == VERSION_GZIP)
				{
					var data = new byte[length - 1];
					file.Read(data);
					var ret =
						new BinaryReader(new GZipStream(new MemoryStream(data), CompressionMode.Decompress));
					Console.WriteLine($"READ {x}, {z} Found");
					return ret;
				}

				if (version == VERSION_DEFLATE)
				{
					var data = new byte[length - 1];
					file.Read(data);
					var ret =
						new BinaryReader(new DeflateStream(new MemoryStream(data), CompressionMode.Decompress));
					Console.WriteLine($"READ {x}, {z} Found");
					return ret;
				}

				Console.WriteLine($"READ {x}, {z} Unknown version " + version);
				return null;
			}
			catch (IOException exception)
			{
				Console.WriteLine($"READ {x}, {z} IOException");
				return null;
			}
		}

		//TODO: Line 247

		public void write(int x, int z, byte[] data, int length)
		{
			try
			{
				var offset = getOffset(x, z);
				var sectorNumber = offset >> 8;
				var sectorsAllocated = offset & 0xFF;
				var sectorsNeeded = (length + CHUNK_HEADER_SIZE) / SECTOR_BYTES + 1;
				if (sectorsNeeded >= 256)
				{
					return;
				}

				if (sectorNumber != 0 && sectorsAllocated == sectorsNeeded)
				{
					//Overwrite Sector
					Console.WriteLine($"WRITE {x}, {z} Rewrite");
					write(sectorNumber, data, length);
				}
				else
				{
					for (var i = 0; i < sectorsAllocated; ++i)
					{
						sectorFree[sectorNumber + i] = true;
					}

					var runStart = sectorFree.IndexOf(true);
					var runLength = 0;
					if (runStart != -1)
					{
						for (var i = runStart; i < sectorFree.Count; ++i)
						{
							if (runLength != 0)
							{
								if (sectorFree[i]) runLength++;
								else runLength = 0;
							}
							else if (sectorFree[i])
							{
								runStart = i;
								runLength = 1;
							}

							if (runLength >= sectorsNeeded)
							{
								break;
							}
						}
					}

					if (runLength >= sectorsNeeded)
					{
						// Found Free Space Reuseing
						Console.WriteLine($"SAVE {x}, {z} Reuse");
						sectorNumber = runStart;
						setOffset(x, z, (sectorNumber << 8) | sectorsNeeded);
						for (var i = 0; i < sectorsNeeded; ++i)
						{
							sectorFree[sectorNumber + i] = false;
						}

						write(sectorNumber, data, length);
					}
					else
					{
						//No Free SPace Grow FIle
						Console.WriteLine($"SAVE {x}, {z} Grow File: {length}");
						file.Seek(file.Length, SeekOrigin.Begin);
						sectorNumber = sectorFree.Count;
						for (var i = 0; i < sectorsNeeded; ++i)
						{
							file.SetLength(file.Length + emptySector.Length);
							sectorFree.Add(false);
							file.Flush();
						}

						sizeDelta += SECTOR_BYTES * sectorsNeeded;
						write(sectorNumber, data, length);
						setOffset(x, z, (sectorNumber << 8) | sectorsNeeded);
					}
				}

				setTimestamp(x, z, (int) (DateTime.Now.Ticks / 1000L));
			}
			catch (IOException exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		private void write(int sectorNumber, byte[] data, int length)
		{
			Console.WriteLine(" " + sectorNumber);
			file.Seek(sectorNumber * SECTOR_BYTES, SeekOrigin.Begin);
			file.Write(BitConverter.GetBytes(length + 1));
			file.Write(BitConverter.GetBytes(VERSION_DEFLATE));
			file.Write(data, 0, length);
			file.Flush();
		}

		public int getSizeDelta()
		{
			var ret = sizeDelta;
			sizeDelta = 0;
			return ret;
		}

		private bool outOfBounds(int x, int z)
		{
			return x < 0 || x >= 32 || z < 0 || z >= 32;
		}

		private int getOffset(int x, int z)
		{
			return offsets[x + z * 32];
		}

		public bool hasChunk(int x, int z)
		{
			return getOffset(x, z) != 0;
		}

		private void setOffset(int x, int z, int offset)
		{
			offsets[x + z * 32] = offset;
			file.Seek((x + z * 32) * 4, SeekOrigin.Begin);
			file.Write(BitConverter.GetBytes(offset));
			file.Flush();
		}

		private void setTimestamp(int x, int z, int value)
		{
			chunkTimestamps[x + z * 32] = value;
			file.Seek(SECTOR_BYTES + (x + z * 32) * 4, SeekOrigin.Begin);
			file.Write(BitConverter.GetBytes(value));
			file.Flush();
		}

		public void close()
		{
			file.Close();
		}
	}
}