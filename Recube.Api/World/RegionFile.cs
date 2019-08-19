using System;
using System.Collections.Generic;
using System.IO;
using fNbt;
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

		/// <summary>
		///     Region File Used to store 32x32 Chunks
		/// </summary>
		/// <param name="regionFile">FileStream of the Region File(r.X.Z.mca)</param>
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

				for (var offsetIndex = 0; offsetIndex < 1024; ++offsetIndex)
				{
					var offsetBuffer = new byte[4];
					this.regionFile.Read(offsetBuffer, 0, 4);
					var offset = BitConverter.ToInt32(offsetBuffer.ChangeEndian(), 0);
					offsets[offsetIndex] = offset;
					if (offset != 0 && (offset >> 8) + (offset & 255) <= sectorsFree.Count)
					{
						for (var l = 0; l < (offset & 255); ++l)
						{
							sectorsFree[(offset >> 8) + l] = false;
						}
					}
				}

				for (var k1 = 0; k1 < 1024; ++k1)
				{
					var timeStempBuffer = new byte[4];
					// Making sure its a Big-Endian
					this.regionFile.Read(timeStempBuffer.ChangeEndian(), 0, 4);
					chunkTimestamps[k1] = BitConverter.ToInt32(timeStempBuffer);
				}
			}
			catch (IOException exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		/// <summary>
		///     Returns a Relative Chunk within the Region
		/// </summary>
		/// <param name="x">Chunk X Position within the Region (0-31)</param>
		/// <param name="z">Chunk Z Position within the Region (0-31)</param>
		/// <returns>Chunk as NbtFile</returns>
		public NbtFile? getChunkData(int x, int z)
		{
			if (OutOfBounds(x, z))
				return null;
			try
			{
				var i = getOffset(x, z);
				if (i == 0)
					return null;

				var j = i >> 8;
				var k = i & 255;
				if (j + k > sectorsFree.Count)
					return null;

				regionFile.Seek(j * 4096, SeekOrigin.Begin);
				var dataLength = new byte[4];
				regionFile.Read(dataLength, 0, 4);
				var l = BitConverter.ToInt32(dataLength.ChangeEndian());
				if (l > 4096 * k)
					return null;

				if (l <= 0)
					return null;

				var compressionType = (byte) regionFile.ReadByte();
				if (compressionType == 1 || compressionType == 2)
				{
					var byte1 = new byte[l - 1];
					regionFile.Read(byte1, 0, l - 1);
					var file = new NbtFile();
					file.LoadFromBuffer(byte1, 0, l - 1, NbtCompression.AutoDetect);
					return file;
				}

				return null;
			}
			catch (IOException)
			{
				return null;
			}
		}

		/// <summary>
		///     Returns if the region contains the chunk
		/// </summary>
		/// <param name="x">Chunk X Position within the Region (0-31)</param>
		/// <param name="z">Chunk Z Position within the Region (0-31)</param>
		/// <returns>If the chunk exist withing the Region</returns>
		public bool DoesChunkExist(int x, int z)
		{
			if (OutOfBounds(x, z))
				return false;
			var Offset = getOffset(x, z);
			if (Offset == 0)
				return false;
			var ROffset = Offset >> 8;
			var AndOffset = Offset & 255;
			if (ROffset + AndOffset > sectorsFree.Count)
				return false;
			try
			{
				regionFile.Seek(ROffset * 4096, SeekOrigin.Begin);
				var timestampBuffer = new byte[4];
				regionFile.Read(timestampBuffer, 0, 4);
				var TimeStamp = BitConverter.ToInt32(timestampBuffer.ChangeEndian());
				if (TimeStamp > 4096 * AndOffset)
					return false;
				return TimeStamp > 0;
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		///     Writes NBT Data to the Chunk X, Z Within the Region
		/// </summary>
		/// <param name="x">Chunk X Position within the Region (0-31)</param>
		/// <param name="z">Chunk Z Position within the Region (0-31)</param>
		/// <param name="data">NBT Data as Byte Array</param>
		/// <param name="length">Length of the NBT Data Written</param>
		public void Write(int x, int z, byte[] data, int length)
		{
			try
			{
				var Offset = getOffset(x, z);
				var ROffset = Offset >> 8;
				var AndOffset = Offset & 255;
				var NeededSectors = (length + 5) / 4096 + 1;
				if (NeededSectors >= 256) // Chunk can not be 1MB or above so just return.
					return;

				if (ROffset != 0 && AndOffset == 1)
				{
					Write(ROffset, data, length);
				}
				else
				{
					for (var i1 = 0; i1 < AndOffset; ++i1)
					{
						sectorsFree[ROffset + i1] = true;
					}

					var firstFreeSector = sectorsFree.IndexOf(true);
					var runAmmount = 0;
					if (firstFreeSector != -1)
					{
						for (var k1 = firstFreeSector; k1 < sectorsFree.Count; ++k1)
						{
							if (runAmmount != 0)
							{
								if (sectorsFree[k1])
								{
									++runAmmount;
								}
								else
								{
									runAmmount = 0;
								}
							}
							else if (sectorsFree[k1])
							{
								firstFreeSector = k1;
								runAmmount = 1;
							}

							if (runAmmount >= 1)
							{
								break;
							}
						}
					}

					if (runAmmount >= 1)
					{
						ROffset = firstFreeSector;
						SetOffset(x, z, firstFreeSector << 8 | NeededSectors);
						for (var NeededSector = 0; NeededSector < NeededSectors; NeededSector++)
						{
							sectorsFree[ROffset + NeededSector] = false;
						}

						Write(ROffset, data, length);
					}
					else
					{
						regionFile.Seek(0, SeekOrigin.End);
						ROffset = sectorsFree.Count;
						for (var NeededSector = 0; NeededSector < NeededSectors; ++NeededSector)
						{
							regionFile.Write(EMPTY_SECTOR);
							sectorsFree.Add(false);
						}

						sizeDelta += 4096 * NeededSectors;
						Write(ROffset, data, length);
						SetOffset(x, z, ROffset << 8 | NeededSectors);
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
			regionFile.Seek(sectorNumber * 4096, SeekOrigin.Begin); // Set Position
			regionFile.Write(BitConverter.GetBytes(length + 1).ChangeEndian()); // Length of Data
			regionFile.WriteByte(2); // Compression Method
			regionFile.Write(data, 0, length); // Write Data
		}

		private bool OutOfBounds(int x, int z)
		{
			return x < 0 || x >= 32 || z < 0 || z >= 32;
		}

		private int getOffset(int x, int z)
		{
			return offsets[x + z * 32];
		}

		/// <summary>
		///     Returns if the Chunk is saved
		/// </summary>
		/// <param name="x">Chunk X Position within the Region (0-31)</param>
		/// <param name="z">Chunk Z Position within the Region (0-31)</param>
		/// <returns>If Chunk is saved</returns>
		public bool IsChunkSaved(int x, int z)
		{
			return getOffset(x, z) != 0;
		}

		private void SetOffset(int x, int z, int offset)
		{
			offsets[x + z * 32] = offset;
			regionFile.Seek((x + z * 32) * 4, SeekOrigin.Begin);
			regionFile.Write(BitConverter.GetBytes(offset).ChangeEndian());
			regionFile.Flush();
		}

		private void SetChunkTimestamp(int x, int z, int timestamp)
		{
			chunkTimestamps[x + z * 32] = timestamp;
			regionFile.Seek(4096 + (x + z * 32) * 4, SeekOrigin.Begin);
			regionFile.Write(BitConverter.GetBytes(timestamp).ChangeEndian());
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