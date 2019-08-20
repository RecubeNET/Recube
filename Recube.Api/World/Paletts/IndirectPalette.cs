using System.Collections.Generic;
using System.Data;
using DotNetty.Buffers;
using Recube.Api.Block;
using Recube.Api.Network.Extensions;

namespace Recube.Api.World.Paletts
{
	public class IndirectPalette : IPalette
	{
		private readonly byte _bitsPerBlock;
		private Dictionary<uint, BlockState> _idToState;
		private Dictionary<BlockState, uint> _stateToId;

		public IndirectPalette(byte palBitsPerBlock)
		{
			_bitsPerBlock = palBitsPerBlock;
			_idToState = new Dictionary<uint, BlockState>();
			_stateToId = new Dictionary<BlockState, uint>();
		}

		public uint IdForState(BlockState state)
		{
			//TODO: Implement
			if (state != null && _stateToId.ContainsKey(state))
				return _stateToId[state];
			return 0;
		}

		public BlockState StateForId(uint id)
		{
			//TODO: Implement
			if (_idToState.ContainsKey(id))
				return _idToState[id];
			return new BlockState(0, false, new Dictionary<string, object>());
		}

		public byte GetBitsPerBlock()
		{
			return _bitsPerBlock;
		}

		public void Read(IByteBuffer data)
		{
			_idToState = new Dictionary<uint, BlockState>();
			_stateToId = new Dictionary<BlockState, uint>();
			// Palette Length
			var length = data.ReadVarInt();
			// Palette
			for (uint id = 0; id < length; id++)
			{
				var stateId = (uint) data.ReadVarInt();
				var state = BlockState.GetStateFromGlobalPaletteId(stateId);
				_idToState[id] = state;
				_stateToId[state] = id;
			}
		}

		public void Write(IByteBuffer data)
		{
			if (_idToState.Count != _stateToId.Count) // both should be equivalent
				throw new EvaluateException("This should be equal");
			// Palette Length
			data.WriteVarInt(_idToState.Count);
			// Palette
			for (uint id = 0; id < _idToState.Count; id++)
			{
				var state = _idToState[id];
				var stateId = BlockState.GetGlobalPaletteIdFromState(state);
				data.WriteVarInt((int) stateId);
			}
		}
	}
}