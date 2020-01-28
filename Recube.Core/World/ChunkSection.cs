using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using Recube.Api.Network.Extensions;

namespace Recube.Core.World
{
    // Thanks to wiki.vg and Glowstone for providing some examples so I could implement this.
    /// <summary>
    ///     This class represents a <see cref="Chunk" /> section in Minecraft.
    ///     It manages choosing the right palette and writing the chunk section data to a Netty ByteBuf.
    ///     It uses two palette strategies:
    ///     The first one is called the global/direct palette. When we need 9 (currently, this value may change) or more bits
    ///     to represent every block in this section, we're using this.
    ///     The second is called a indirect palette, which gets used when we need less than 9 bits to represent every state.
    ///     For further information: https://wiki.vg/Chunk_Format
    /// </summary>
    public class ChunkSection
    {
        /// <summary>
        ///     This is the size when the global palette is used
        /// </summary>
        public const int GlobalPaletteBitsPerBlock = 14;

        /// <summary>
        ///     The section width in x as well as z direction
        /// </summary>
        public const int ChunkSectionWidth = 16;

        /// <summary>
        ///     The section height in y direction
        /// </summary>
        public const int ChunkSectionHeight = 16;

        /// <summary>
        ///     The total amount of blocks (including air) in this section
        /// </summary>
        public const int DataArraySize = ChunkSectionWidth * ChunkSectionWidth * ChunkSectionHeight;

        /// <summary>
        ///     When exceeding this value, the global palette will be used
        /// </summary>
        public const int UseGlobalPaletteBits = 8;

        /// <summary>
        ///     Indicator how many non-air blocks exist in this section
        /// </summary>
        private short _blockCount;

        /// <summary>
        ///     This is a possible indirect palette. If it's null then the global palette is used instead.
        /// </summary>
        private List<int>? _palette;

        private ChunkSection(short blockCount, VariableBlockArray array, List<int>? palette)
        {
            _blockCount = blockCount;
            Data = array;
            _palette = palette;
        }

        /// <summary>
        ///     This array holds every block in this section and manages the bit shifting stuff
        /// </summary>
        public VariableBlockArray Data { get; private set; }

        /// <summary>
        ///     Factory function to initialize a new chunk section
        /// </summary>
        /// <param name="types">All blocks in this section</param>
        /// <returns>The built section</returns>
        /// <exception cref="InvalidOperationException">
        ///     If the length of the types array doesn't match <see cref="DataArraySize" />
        /// </exception>
        public static ChunkSection Build(int[] types)
        {
            if (types.Length != DataArraySize)
                throw new InvalidOperationException(
                    $"types' length ({types.Length}) must equal DataArraySize({DataArraySize})");

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
            for (var i = 0; i < vba.Capacity; i++) vba.Set(i, palette?.IndexOf(types[i]) ?? types[i]);

            return new ChunkSection((short) blockCount, vba, palette);
        }

        /// <summary>
        ///     Recount how many non-air blocks exist in the this chunk section.
        ///     Sets the value into the <see cref="_blockCount" /> variable
        /// </summary>
        public void Recount()
        {
            _blockCount = 0;
            for (var i = 0; i < DataArraySize; i++)
            {
                var type = (int) Data.Get(i); // CAST TO INT
                if (_palette != null) type = _palette[type];

                if (type != 0) _blockCount++;
            }
        }

        /// <summary>
        ///     Reconstructs this ChunkSection.
        ///     This will check if it would be beneficial to use a indirect palette or make it smaller.
        ///     Do not call this too often as it is a heavy method
        /// </summary>
        public void Optimize()
        {
            var optimized = Build(GetAllTypes());
            _blockCount = optimized._blockCount;
            _palette = optimized._palette;
            Data = optimized.Data;
        }

        /// <summary>
        ///     Gets all raw types from this chunk section
        /// </summary>
        /// <returns>An array containing every block</returns>
        public int[] GetAllTypes()
        {
            var types = new int[DataArraySize];
            for (var i = 0; i < DataArraySize; i++)
            {
                var type = (int) Data.Get(i); // CAST TO INT
                if (_palette != null) type = _palette[type];
                types[i] = type;
            }

            return types;
        }

        /// <summary>
        ///     Sets a block type in this section
        /// </summary>
        /// <param name="x">The x coordinate in this section (currently 0..16)</param>
        /// <param name="y">The y coordinate in this section (currently 0..16)</param>
        /// <param name="z">The z coordinate in this section (currently 0..16)</param>
        /// <param name="type">The type</param>
        /// <exception cref="InvalidOperationException">If one of the coordinates is out of bounds</exception>
        /// <exception cref="OverflowException">If type would need more bits than GlobalPaletteBitsPerBlock</exception>
        public void SetType(int x, int y, int z, int type)
        {
            if (VariableBlockArray.NeededBits(type) > GlobalPaletteBitsPerBlock)
                throw new OverflowException(
                    $"type {type} would need more bits({VariableBlockArray.NeededBits(type)}) than the current limit {GlobalPaletteBitsPerBlock}");

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

                    // CHECK IF PALETTE WOULD NEED TO RESIZE
                    if ((ulong) typeToSet > Data.MaxSize())
                    {
                        var neededBits = VariableBlockArray.NeededBits(typeToSet);

                        // CHECK IF WE SHOULD JUST USE THE GLOBAL PALETTE INSTEAD
                        if (neededBits > UseGlobalPaletteBits)
                        {
                            // Use the global palette
                            Data = Data.Resize(GlobalPaletteBitsPerBlock);
                            for (var i = 0; i < Data.Capacity; i++)
                            {
                                var paletteVal = _palette[(int) Data.Get(i)]; // CASTING TO INT
                                Data.Set(i, paletteVal);
                            }

                            _palette = null;
                            typeToSet = type;
                        }
                        else
                        {
                            // We're under the value of UseGlobalPalette and just resizing.
                            Data = Data.Resize(neededBits);
                        }
                    }
                }
                else
                {
                    typeToSet = paletteIndex;
                }
            }

            // Sets the type
            Data.Set(Index(x, y, z), typeToSet);
        }

        /// <summary>
        ///     Gets the raw type from the position
        /// </summary>
        /// <param name="x">The x coordinate in this section (currently 0..16)</param>
        /// <param name="y">The y coordinate in this section (currently 0..16)</param>
        /// <param name="z">The z coordinate in this section (currently 0..16)</param>
        /// <returns>The type</returns>
        /// <exception cref="InvalidOperationException">If one of the coordinates is out of bounds</exception>
        public int GetType(int x, int y, int z)
        {
            var val = (int) Data.Get(Index(x, y, z)); // CAST TO INT. MAYBE THIS WILL BREAK SOMEDAY
            if (_palette != null) val = _palette[val];

            return val;
        }

        /// <summary>
        ///     Transforms the x, y & z values into one block index
        /// </summary>
        /// <param name="x">The x coordinate in this section (currently 0..16)</param>
        /// <param name="y">The y coordinate in this section (currently 0..16)</param>
        /// <param name="z">The z coordinate in this section (currently 0..16)></param>
        /// <returns>The type</returns>
        /// <exception cref="InvalidOperationException">When one of the coordinates is out of bounds</exception>
        public int Index(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x > ChunkSectionWidth || z > ChunkSectionWidth || y > ChunkSectionHeight)
                throw new InvalidOperationException($"indexed type is out of bounds: x = {x} y = {y} z = {z}");
            return ((y & 0xF) << 8) | (z << 4) | x;
        }

        /// <summary>
        ///     Writes this chunk section data into a <see cref="IByteBuffer" />
        /// </summary>
        /// <param name="buf">The byte buffer</param>
        public void Serialize(IByteBuffer buf)
        {
            buf.WriteShort(_blockCount);
            buf.WriteByte(Data.BitsPerValue);
            if (_palette != null)
            {
                buf.WriteVarInt(_palette.Count);
                buf.WriteVarIntArray(_palette);
            }

            buf.WriteVarInt(Data.ResultingLongs.Length);
            buf.WriteLongArray(Data.ResultingLongs);
        }
    }
}