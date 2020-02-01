using Recube.Api.World;

namespace Recube.Api.Block
{
    /// <summary>
    /// This class represents the main class of each existing block.
    /// Each inheritor needs to fulfill certain requirements to be parsed successfully.
    /// Firstly, the <see cref="BlockAttribute"/> needs to be set with the name of the block
    /// Secondly, if the block has more than one state, all properties must be specified within the class (as nested types) and be marked with <see cref="PropertyStateAttribute"/>.
    /// Each value of these enums need to have a <see cref="PropertyConditionAttribute"/> stating what condition needs to be met to become the value.
    /// Also, exactly one field must represent each of these enums in the derived class
    /// And lastly, there needs to be a constructor which only parameters are the fields of <see cref="PropertyStateAttribute"/> (properties).
    /// 
    /// For an example visit: <see cref="Recube.Api.Block.Impl.ExampleBlock"/>
    /// </summary>
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

        /// <inheritdoc cref="IBlockStateRegistry.GetStateByBaseBlock"/>
        public BlockState? AsBlockState()
            => RecubeApi.Recube.GetBlockStateRegistry().GetStateByBaseBlock(this);
    }
}