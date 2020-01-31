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
            var parsedBlock = _parsedBlocks.First(b => b.BaseBlock == block.GetType());
            if (parsedBlock == null) return null;

            var blockStates = _blockStates[parsedBlock.Name];
            if (blockStates == null || blockStates.Count == 0) return null;


            foreach (var parsedBlockProperty in parsedBlock.Properties)
            {
                var name = parsedBlockProperty.PropertyName;
                var val = parsedBlockProperty.Field.GetValue(block);
                blockStates = blockStates.Where(blockState => string.Equals(blockState.Properties[name],
                    parsedBlockProperty.Conditions[(int) val].ToString(), StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (blockStates.Count == 0)
            {
                return _blockStates[parsedBlock.Name].First(s => s.Default);
            }

            return blockStates[0];
        }

        public BlockState? GetBlockStateByNetworkId(int id)
        {
            return _blockStates.Select(s => s.Value)
                .SelectMany(blockStates => blockStates.Where(blockState => blockState.NetworkId == id))
                .FirstOrDefault();
        }
    }
}