using System;
using System.Collections.Generic;
using System.IO;
using Recube.Api.Network.Extensions;

namespace Recube.Api.World
{
	public class RegionFile
	{
		private static readonly byte[] EMPTY_SECTOR = new byte[4096];
		private readonly int[] chunkTimestamps = new int[1024];
		private readonly int[] offsets = new int[1024];
		private readonly FileStream regionFile;
		private readonly List<bool> sectorsFree;
		private long lastModified;
		private int sizeDelta;

		public RegionFile(FileStream regionFile)
		{
			this.regionFile = regionFile;
			sizeDelta = 0;
			try
			{
				if (File.Exists(this.regionFile.Name))
				{
					lastModified = File.GetLastWriteTime(this.regionFile.Name).Ticks;
				}

				// Write header if not pressent
				if (this.regionFile.Length < 4096L)
				{
					this.regionFile.SetLength(this.regionFile.Length + 4096L * 2);
					sizeDelta += 8192;
				}

				// File Padding
				if ((this.regionFile.Length & 4095L) != 0L)
				{
					for (var i = 0; (long) i < (this.regionFile.Length & 4095L); ++i)
					{
						this.regionFile.WriteByte(0);
					}
				}

				// Create Free Sector List
				var i1 = (int) this.regionFile.Length / 4096;
				sectorsFree = new List<bool>();

				for (var i = 0; i < i1; ++i)
				{
					sectorsFree.Add(true);
				}

				sectorsFree[0] = false;
				sectorsFree[1] = false;
				//Reset Pointer
				this.regionFile.Position = 0;

				for (var j1 = 0; j1 < 1024; ++j1)
				{
					var offsetBuffer = new byte[4];
					this.regionFile.Read(offsetBuffer, 0, 4);
					// Making sure its a Big-Endian
					var k = BitConverter.ToInt32(offsetBuffer.ToBigEndian(), 0);
					offsets[j1] = k;
					if (k != 0 && (k >> 8) + (k & 255) <= sectorsFree.Count)
					{
						for (var l = 0; l < (k & 255); ++l)
						{
							sectorsFree[(k >> 8) + 1] = false;
						}
					}
				}

				for (var k1 = 0; k1 < 1024; ++k1)
				{
					var timeStempBuffer = new byte[4];
					// Making sure its a Big-Endian
					this.regionFile.Read(timeStempBuffer.ToBigEndian(), 0, 4);
					chunkTimestamps[k1] = BitConverter.ToInt32(timeStempBuffer);
				}
			}
			catch (IOException exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		public void Write(int x, int z, byte[] data, int length)
		{
			try
			{
				//Offset
				var i = getOffset(x, z);
				//Sector Number
				var j = i >> 8;
				//FreeSectors??
				var k = i & 255;
				var l = (length + 5) / 4096 + 1;
				if (l >= 256)
					return;

				if (j != 0 && k == 1)
				{
					Write(j, data, length);
				}
				else
				{
					for (var i1 = 0; i1 < k; ++i1)
					{
						sectorsFree[j + i1] = true;
					}

					var l1 = sectorsFree.IndexOf(true);
					var j1 = 0;
					if (l1 != -1)
					{
						for (var k1 = l1; k1 < sectorsFree.Count; ++k1)
						{
							if (j1 != 0)
							{
								if (sectorsFree[k1])
								{
									++j1;
								}
								else
								{
									j1 = 0;
								}
							}
							else if (sectorsFree[k1])
							{
								l1 = k1;
								j1 = 1;
							}

							if (j1 >= 1)
							{
								break;
							}
						}
					}

					if (j1 >= 1)
					{
						j = l1;
						SetOffset(x, z, l1 << 8 | l);
						for (var j2 = 0; j2 < l; j2++)
						{
							sectorsFree[j + j2] = false;
						}

						Write(j, data, length);
					}
					else
					{
						regionFile.Seek(0, SeekOrigin.End);
						j = sectorsFree.Count;
						for (var i2 = 0; i2 < l; ++i2)
						{
							regionFile.Write(EMPTY_SECTOR);
							sectorsFree.Add(false);
						}

						sizeDelta += 4096 * l;
						Write(j, data, length);
						SetOffset(x, z, j << 8 | l);
					}
				}

				SetChunkTimestamp(x, z, (int) DateTime.Now.Ticks / 1000);
			}
			catch (IOException exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		private void Write(int sectorNumber, byte[] data, int length)
		{
			regionFile.Seek(sectorNumber * 4096, SeekOrigin.Begin);
			regionFile.Write(BitConverter.GetBytes(length + 1).ToBigEndian());
			regionFile.WriteByte(2);
			regionFile.Write(data, 0, length);
		}

		public bool OutOfBounds(int x, int z)
		{
			return x < 0 || x >= 32 || z < 0 || z >= 32;
		}

		private int getOffset(int x, int z)
		{
			return offsets[x + z * 32];
		}

		public bool IsChunkSaved(int x, int z)
		{
			return getOffset(x, z) != 0;
		}

		public void SetOffset(int x, int z, int offset)
		{
			offsets[x + z * 32] = offset;
			regionFile.Seek((x + z * 32) * 4, SeekOrigin.Begin);
			// Making sure its a Big-Endian
			regionFile.Write(BitConverter.GetBytes(offset).ToBigEndian());
			regionFile.Flush();
		}

		public void SetChunkTimestamp(int x, int z, int timestamp)
		{
			chunkTimestamps[x + z * 32] = timestamp;
			regionFile.Seek(4096 + (x + z * 32) * 4, SeekOrigin.Begin);
			// Making sure its a Big-Endian
			//660309
			regionFile.Write(BitConverter.GetBytes(timestamp).ToBigEndian());
			regionFile.Flush();
		}

		public void Close()
		{
			//TODO: Improve this shit.
			if (regionFile != null)
			{
				regionFile.Flush();
				regionFile.Close();
				regionFile.DisposeAsync();
			}
		}
	}
}