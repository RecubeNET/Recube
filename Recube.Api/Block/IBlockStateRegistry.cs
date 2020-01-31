namespace Recube.Api.Block
{
    public interface IBlockStateRegistry
    {
        BlockState? GetStateByBaseBlock(BaseBlock block);
        BlockState? GetBlockStateByNetworkId(int id);
    }
}