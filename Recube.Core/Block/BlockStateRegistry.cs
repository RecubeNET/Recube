using System;
using System.Collections.Generic;
using System.Linq;
using Recube.Api.Block;

namespace Recube.Core.Block
{
    public class BlockStateRegistry : IBlockStateRegistry
    {
        private readonly Dictionary<string, List<BlockState>> _blockStates;
        private readonly List<ParsedBlock> _parsedBlocks;

        public BlockStateRegistry(Dictionary<string, List<BlockState>> blockStates, List<ParsedBlock> parsedBlocks)
        {
            _blockStates = blockStates;
            _parsedBlocks = parsedBlocks;
        }

        public BlockState? GetStateByBaseBlock(BaseBlock block)
        {
            var parsedBlock = _parsedBlocks.First(b => b.BaseBlockType == block.GetType());
            if (parsedBlock == null) return null;

            var blockStates = _blockStates[parsedBlock.Name];
            if (blockStates == null || blockStates.Count == 0) return null;


            foreach (var parsedBlockProperty in parsedBlock.Properties)
            {
                var name = parsedBlockProperty.PropertyName;
                var val = parsedBlockProperty.Field.GetValue(block);
                blockStates = blockStates.Where(blockState => string.Equals(blockState.Properties[name],
                    parsedBlockProperty.Conditions[(int) val], StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return blockStates.Count == 0 ? _blockStates[parsedBlock.Name].First(s => s.Default) : blockStates[0];
        }

        public BlockState? GetBlockStateByNetworkId(int id)
        {
            return _blockStates.Select(s => s.Value)
                .SelectMany(blockStates => blockStates.Where(blockState => blockState.NetworkId == id))
                .FirstOrDefault();
        }

        public BaseBlock? GetBaseBlockFromState(BlockState state)
        {
            var parsedBlock = _parsedBlocks.FirstOrDefault(b => b.Name == state.BaseName);
            if (parsedBlock == null) return null;

            var parameters = new List<object>();

            foreach (var parsedProperty in parsedBlock.Properties)
            {
                var property = state.Properties[parsedProperty.PropertyName];
                if (property == null) return null;
                var (key, value) = parsedProperty.Conditions.FirstOrDefault(kvp =>
                    string.Equals(kvp.Value.ToString(), property, StringComparison.OrdinalIgnoreCase));
                if (value == null) return null;
                parameters.Add(Enum.ToObject(parsedProperty.Type, key));
            }

            return (BaseBlock) parsedBlock.ConstructorInfo.Invoke(parameters.ToArray());
        }

        public BlockState? FromRaw(string name, Dictionary<string, string> properties)
        {
            if (!_blockStates.ContainsKey(name)) return null;

            var blockStates = _blockStates[name];
            if (blockStates == null || blockStates.Count == 0) return null;

            foreach (var property in properties)
                blockStates = blockStates.Where(bs => string.Equals(bs.Properties.GetValueOrDefault(property.Key) ?? "",
                    property.Value,
                    StringComparison.OrdinalIgnoreCase)).ToList();

            return blockStates.Count == 0 ? _blockStates[name].First(s => s.Default) : blockStates[0];
        }
    }
}