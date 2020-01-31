using Recube.Api.World;

namespace Recube.Api.Block
{
    public abstract class BaseBlock
    {
        /// <remarks>
        /// This value will be initialized when the Block is placed in a world.
        /// </remarks>
        public string Name { get; internal set; }

        /// <remarks>
        /// This value will be initialized when the Block is placed in a world.
        /// </remarks>
        public IWorld World { get; internal set; }

        /// <remarks>
        /// This value will be initialized when the Block is placed in a world.
        /// </remarks>
        public Location Location { get; internal set; }

        public BlockState AsBlockState()
            => RecubeApi.Recube.GetBlockStateRegistry().GetStateByBaseBlock(this);
    }
}