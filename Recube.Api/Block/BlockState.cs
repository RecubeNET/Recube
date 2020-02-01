using System.Collections.Generic;

namespace Recube.Api.Block
{
    /// <summary>
    /// This class represents a BlockState as by its definition in Mojang's generated blocks.json
    /// </summary>
    public class BlockState
    {
        /// <summary>
        /// Whether this state is the default one.
        /// This will be used by <see cref="IBlockStateRegistry"/> when the <see cref="Properties"/> of this state don't match with any known states
        /// </summary>
        public bool Default { get; }

        /// <summary>
        /// The network id of this state.
        /// Mostly used internally.
        /// </summary>
        public int NetworkId { get; }

        /// <summary>
        /// The base block name. For example "minecraft:grass_block"
        /// </summary>
        public string BaseName { get; }

        /// <summary>
        /// This represents all "conditions" to become this state
        /// </summary>
        // PROPERTY NAME, PROPERTY CONDITION
        public Dictionary<string, string>? Properties { get; }

        public BlockState(string baseName, int networkId, bool @default, Dictionary<string, string>? properties)
        {
            BaseName = baseName;
            NetworkId = networkId;
            Properties = properties;
            Default = @default;
        }

        /// <inheritdoc cref="IBlockStateRegistry.GetBaseBlockFromState"/>
        public BaseBlock? AsBlock() => RecubeApi.Recube.GetBlockStateRegistry().GetBaseBlockFromState(this);
    }
}