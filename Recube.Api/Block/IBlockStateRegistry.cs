using System.Collections.Generic;

namespace Recube.Api.Block
{
    public interface IBlockStateRegistry
    {
        /// <summary>
        /// Gets the BlockState of the current block
        /// </summary>
        /// <param name="block">The block</param>
        /// <returns>The BlockState, it's default or, if the block doesn't exist in the list, null</returns>
        BlockState? GetStateByBaseBlock(BaseBlock block);

        /// <summary>
        /// Gets the BlockState from the given id
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>Returns null if the id could not be found</returns>
        BlockState? GetBlockStateByNetworkId(int id);

        /// <summary>
        /// Gets the BlockState from the name and the required properties which need to be met.
        /// </summary>
        /// <param name="name">The block name. For example "minecraft:grass_block"</param>
        /// <param name="properties">A dictionary with each property. For example {"snowy": "true"}</param>
        /// <returns>The BlockState or null if the name could not be found (or it's default state)</returns>
        public BlockState? FromRaw(string name, Dictionary<string, string> properties);

        /// <summary>
        /// Tries to build a <see cref="BaseBlock"/> with the given state.
        /// </summary>
        /// <param name="state">The state</param>
        /// <returns>A new BaseBlock (without Location data etc.) or null</returns>
        public BaseBlock? GetBaseBlockFromState(BlockState state);
    }
}