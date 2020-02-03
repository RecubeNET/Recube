using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Faulty
{
    [Block("recube:testblock")]
    public class TestBlock : BaseBlock
    {
        // THIS CONSTRUCTOR SHOULD BE INVALID
        public TestBlock(int a)
        {
        }
    }
}