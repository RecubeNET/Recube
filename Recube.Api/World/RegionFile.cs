using System;
using System.Collections.Generic;
using System.IO;
using fNbt;
using Recube.Api.Network.Extensions;

namespace Recube.Api.World
{
    public class RegionFile
    {
        private static readonly byte[] _emptySector = new byte[4096];
        private readonly int[] _chunkTimestamps = new int[1024];
        private readonly int[] _offsets = new int[1024];
        private readonly FileStream _regionFile;
        private readonly List<bool> _sectorsFree;
        private long _lastModified;
        private int _sizeDelta;

        /// <summary>
        ///     Region File Used to store 32x32 Chunks
        /// </summary>
        /// <param name="regionFile">FileStream of the Region File(r.X.Z.mca)</param>
        public RegionFile(FileStream regionFile)
        {
            _regionFile = regionFile;
            _sizeDelta = 0;
            try
            {
                if (File.Exists(_regionFile.Name))
                {
                    _lastModified = File.GetLastWriteTime(_regionFile.Name).Ticks;
                }

                // Write header if not present
                if (_regionFile.Length < 4096L)
                {
                    _regionFile.SetLength(_regionFile.Length + 4096L * 2);
                    _sizeDelta += 8192;
                }

                // File Padding
                if ((_regionFile.Length & 4095L) != 0L)
                {
                    for (var i = 0; (long) i < (_regionFile.Length & 4095L); ++i)
                    {
                        _regionFile.WriteByte(0);
                    }
                }

                // Create Free Sector List
                var i1 = (int) _regionFile.Length / 4096;
                _sectorsFree = new List<bool>();

                for (var i = 0; i < i1; ++i)
                {
                    _sectorsFree.Add(true);
                }

                _sectorsFree[0] = false;
                _sectorsFree[1] = false;
                //Reset Pointer
                _regionFile.Position = 0;

                for (var offsetIndex = 0; offsetIndex < 1024; ++offsetIndex)
                {
                    var offsetBuffer = new byte[4];
                    _regionFile.Read(offsetBuffer, 0, 4);
                    var offset = BitConverter.ToInt32(offsetBuffer.ChangeEndian(), 0);
                    _offsets[offsetIndex] = offset;
                    if (offset == 0 || (offset >> 8) + (offset & 255) > _sectorsFree.Count) continue;
                    for (var l = 0; l < (offset & 255); ++l)
                    {
                        _sectorsFree[(offset >> 8) + l] = false;
                    }
                }

                for (var k1 = 0; k1 < 1024; ++k1)
                {
                    var timeStampBuffer = new byte[4];
                    // Making sure its a Big-Endian
                    _regionFile.Read(timeStampBuffer.ChangeEndian(), 0, 4);
                    _chunkTimestamps[k1] = BitConverter.ToInt32(timeStampBuffer);
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
        public NbtFile? GetChunkData(int x, int z)
        {
            if (OutOfBounds(x, z))
                return null;
            try
            {
                var i = GetOffset(x, z);
                if (i == 0)
                    return null;

                var j = i >> 8;
                var k = i & 255;
                if (j + k > _sectorsFree.Count)
                    return null;

                _regionFile.Seek(j * 4096, SeekOrigin.Begin);
                var dataLength = new byte[4];
                _regionFile.Read(dataLength, 0, 4);
                var l = BitConverter.ToInt32(dataLength.ChangeEndian());
                if (l > 4096 * k)
                    return null;

                if (l <= 0)
                    return null;

                var compressionType = (byte) _regionFile.ReadByte();
                if (compressionType != 1 && compressionType != 2) return null;
                
                var byte1 = new byte[l - 1];
                _regionFile.Read(byte1, 0, l - 1);
                var file = new NbtFile();
                file.LoadFromBuffer(byte1, 0, l - 1, NbtCompression.AutoDetect);
                return file;

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
            var offset = GetOffset(x, z);
            if (offset == 0)
                return false;
            var rOffset = offset >> 8;
            var andOffset = offset & 255;
            if (rOffset + andOffset > _sectorsFree.Count)
                return false;
            try
            {
                _regionFile.Seek(rOffset * 4096, SeekOrigin.Begin);
                var timestampBuffer = new byte[4];
                _regionFile.Read(timestampBuffer, 0, 4);
                var timeStamp = BitConverter.ToInt32(timestampBuffer.ChangeEndian());
                if (timeStamp > 4096 * andOffset)
                    return false;
                return timeStamp > 0;
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
                var offset = GetOffset(x, z);
                var rOffset = offset >> 8;
                var andOffset = offset & 255;
                var neededSectors = (length + 5) / 4096 + 1;
                if (neededSectors >= 256) // Chunk cannot be 1 Megabyte or above so just return.
                    return;

                if (rOffset != 0 && andOffset == 1)
                {
                    Write(rOffset, data, length);
                }
                else
                {
                    for (var i1 = 0; i1 < andOffset; ++i1)
                    {
                        _sectorsFree[rOffset + i1] = true;
                    }

                    var firstFreeSector = _sectorsFree.IndexOf(true);
                    var runAmount = 0;
                    if (firstFreeSector != -1)
                    {
                        for (var k1 = firstFreeSector; k1 < _sectorsFree.Count; ++k1)
                        {
                            if (runAmount != 0)
                            {
                                if (_sectorsFree[k1])
                                {
                                    ++runAmount;
                                }
                                else
                                {
                                    runAmount = 0;
                                }
                            }
                            else if (_sectorsFree[k1])
                            {
                                firstFreeSector = k1;
                                runAmount = 1;
                            }

                            if (runAmount >= 1)
                            {
                                break;
                            }
                        }
                    }

                    if (runAmount >= 1)
                    {
                        rOffset = firstFreeSector;
                        SetOffset(x, z, firstFreeSector << 8 | neededSectors);
                        for (var neededSector = 0; neededSector < neededSectors; neededSector++)
                        {
                            _sectorsFree[rOffset + neededSector] = false;
                        }

                        Write(rOffset, data, length);
                    }
                    else
                    {
                        _regionFile.Seek(0, SeekOrigin.End);
                        rOffset = _sectorsFree.Count;
                        for (var neededSector = 0; neededSector < neededSectors; ++neededSector)
                        {
                            _regionFile.Write(_emptySector);
                            _sectorsFree.Add(false);
                        }

                        _sizeDelta += 4096 * neededSectors;
                        Write(rOffset, data, length);
                        SetOffset(x, z, rOffset << 8 | neededSectors);
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
            _regionFile.Seek(sectorNumber * 4096, SeekOrigin.Begin); // Set Position
            _regionFile.Write(BitConverter.GetBytes(length + 1).ChangeEndian()); // Length of Data
            _regionFile.WriteByte(2); // Compression Method
            _regionFile.Write(data, 0, length); // Write Data
        }

        private static bool OutOfBounds(int x, int z)
        {
            return x < 0 || x >= 31 || z < 0 || z >= 31;
        }

        private int GetOffset(int x, int z)
        {
            return _offsets[x + z * 32];
        }

        /// <summary>
        ///     Returns if the Chunk is saved.
        /// </summary>
        /// <param name="x">Chunk X Position within the Region (0-31)</param>
        /// <param name="z">Chunk Z Position within the Region (0-31)</param>
        /// <returns>If Chunk is saved</returns>
        public bool IsChunkSaved(int x, int z)
        {
            return GetOffset(x, z) != 0;
        }

        private void SetOffset(int x, int z, int offset)
        {
            _offsets[x + z * 32] = offset;
            _regionFile.Seek((x + z * 32) * 4, SeekOrigin.Begin);
            _regionFile.Write(BitConverter.GetBytes(offset).ChangeEndian());
            _regionFile.Flush();
        }

        private void SetChunkTimestamp(int x, int z, int timestamp)
        {
            _chunkTimestamps[x + z * 32] = timestamp;
            _regionFile.Seek(4096 + (x + z * 32) * 4, SeekOrigin.Begin);
            _regionFile.Write(BitConverter.GetBytes(timestamp).ChangeEndian());
            _regionFile.Flush();
        }

        public void Close()
        {
            //TODO: Improve this.
            if (_regionFile == null) return;
            _regionFile.Flush();
            _regionFile.Close();
            _regionFile.DisposeAsync();
        }
    }
}