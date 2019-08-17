using System.Collections.Generic;
using System.Data;
using DotNetty.Buffers;
using Recube.Api.Block;
using Recube.Api.Network.Extensions;

namespace Recube.Core.World.Paletts
{
	public class IndirectPalette : Palette
	{
		private readonly byte bitsPerBlock;
		private Dictionary<uint, BlockState> idToState;
		private Dictionary<BlockState, uint> stateToId;

		public IndirectPalette(byte palBitsPerBlock)
		{
			bitsPerBlock = palBitsPerBlock;
			idToState = new Dictionary<uint, BlockState>();
			stateToId = new Dictionary<BlockState, uint>();
		}

		public uint IdForState(BlockState state)
		{
			//TODO: Implement
			if (state != null && stateToId.ContainsKey(state))
				return stateToId[state];
			return 0;
		}

		public BlockState StateForId(uint id)
		{
			//TODO: Implement
			if (idToState.ContainsKey(id))
				return idToState[id];
			return new BlockState(0, false, new Dictionary<string, object>());
		}

		public byte GetBitsPerBlock()
		{
			return bitsPerBlock;
		}

		public void Read(IByteBuffer data)
		{
			idToState = new Dictionary<uint, BlockState>();
			stateToId = new Dictionary<BlockState, uint>();
			// Palette Length
			var length = data.ReadVarInt();
			// Palette
			for (uint id = 0; id < length; id++)
			{
				var stateId = (uint) data.ReadVarInt();
				var state = BlockState.GetStateFromGlobalPaletteID(stateId);
				idToState[id] = state;
				stateToId[state] = id;
			}
		}

		public void Write(IByteBuffer data)
		{
			if (idToState.Count != stateToId.Count) // both should be equivalent
				throw new EvaluateException("This should be equal");
			// Palette Length
			data.WriteVarInt(idToState.Count);
			// Palette
			for (uint id = 0; id < idToState.Count; id++)
			{
				var state = idToState[id];
				var stateId = BlockState.GetGlobalPaletteIDFromState(state);
				data.WriteVarInt((int) stateId);
			}
		}
	}
}