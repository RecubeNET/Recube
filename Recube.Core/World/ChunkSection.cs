using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using Recube.Api.Network.Extensions;

namespace Recube.Core.World
{
    // Thanks to wiki.vg and Glowstone for providing some examples so I could implement this.
    public class ChunkSection
    {
        public const int GlobalPaletteBitsPerBlock = 14;
        public const int ChunkWidth = 16;
        public const int ChunkHeight = 16;
        public const int DataArraySize = ChunkWidth * ChunkWidth * ChunkHeight;

        /// <summary>
        /// When exceeding this value, the global palette will be used
        /// </summary>
        public const int UseGlobalPaletteBits = 8;

        private short _blockCount;
        private VariableBlockArray _data;
        private List<int>? _palette;

        private ChunkSection(short blockCount, VariableBlockArray array, List<int>? palette)
        {
            _blockCount = blockCount;
            _data = array;
            _palette = palette;
        }

        public static ChunkSection Build(int[] types)
        {
            uint blockCount = 0;
            var palette = new List<int>();
            foreach (var type in types)
            {
                if (type != 0) blockCount++;
                if (!palette.Contains(type)) palette.Add(type);
            }

            var neededBitsPerBlock = VariableBlockArray.NeededBits(palette.Count);
            if (neededBitsPerBlock < 4) neededBitsPerBlock = 4;
            if (neededBitsPerBlock > UseGlobalPaletteBits)
            {
                neededBitsPerBlock = GlobalPaletteBitsPerBlock;
                palette = null;
            }

            var vba = new VariableBlockArray(neededBitsPerBlock, DataArraySize);
            for (var i = 0; i < vba.Capacity; i++)
            {
                vba.Set(i, palette?.IndexOf(types[i]) ?? types[i]);
            }
            
            return new ChunkSection((short) blockCount, vba, palette);
        }

        public void Recount()
        {
            _blockCount = 0;
            for (var i = 0; i < DataArraySize; i++)
            {
                var type = (int) _data.Get(i); // CAST TO INT
                if (_palette != null) type = _palette[type];

                if (type != 0) _blockCount++;
            }
        }

        public void Optimize()
        {

            var optimized = Build(GetAllTypes());
            _blockCount = optimized._blockCount;
            _palette = optimized._palette;
            _data = optimized._data;
        }

        public int[] GetAllTypes()
        {
            var types = new int[DataArraySize];
            for (var i = 0; i < DataArraySize; i++)
            {
                var type = (int) _data.Get(i); // CAST TO INT
                if (_palette != null) type = _palette[type];
                types[i] = type;
            }

            return types;
        }

        public void SetType(int x, int y, int z, int type)
        {
            var oldType = GetType(x, y, z);
            if (oldType != 0) _blockCount--; // CHECK IF NOT AIR
            if (type != 0) _blockCount++;

            var typeToSet = type;

            // CHECK IF PALETTE EXISTS
            if (_palette != null)
            {
                var paletteIndex = _palette.FindIndex(i => i == type);
                if (paletteIndex == -1) // IF TYPE COULD NOT BE FOUND
                {
                    _palette.Add(type);
                    typeToSet = _palette.Count - 1;

                    if (typeToSet > _data.MaxSize())
                    {
                        var neededBits = VariableBlockArray.NeededBits(typeToSet);
                        if (neededBits > UseGlobalPaletteBits)
                        {
                            _data = _data.Resize(GlobalPaletteBitsPerBlock);
                            for (var i = 0; i < _data.Capacity; i++)
                            {
                                var paletteVal = _palette[(int) _data.Get(i)]; // CASTING TO INT
                                _data.Set(i, paletteVal);
                            }

                            _palette = null;
                            typeToSet = type;
                        }
                        else
                        {
                            _data = _data.Resize(neededBits);
                        }
                    }
                }
                else
                {
                    typeToSet = paletteIndex;
                }
            }

            _data.Set(Index(x, y, z), typeToSet);
        }

        public int GetType(int x, int y, int z)
        {
            var val = (int) _data.Get(Index(x, y, z)); // CAST TO INT. MAYBE THIS WILL BREAK SOMEDAY
            if (_palette != null)
            {
                val = _palette[val];
            }

            return val;
        }

        public int Index(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x > ChunkWidth || z > ChunkWidth)
                throw new InvalidOperationException($"indexed type is out of bounds: x = {x} y = {y} z = {z}");
            return (y & 0xF) << 8 | z << 4 | x;
        }

        public void Serialize(IByteBuffer buf)
        {
            buf.WriteShort(_blockCount);
            buf.WriteByte(_data.BitsPerValue);
            if (_palette != null)
            {
                buf.WriteVarInt(_palette.Count);
                buf.WriteVarIntArray(_palette);
            }
            buf.WriteVarInt(_data.ResultingLongs.Length);
            buf.WriteLongArray(_data.ResultingLongs);
        }
    }
}