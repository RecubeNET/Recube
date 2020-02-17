using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Inherit
{
    [Block("recube:testblock")]
    public class TestBlock : ParentTrait
    {
        public ColorEnum Color { get; set; }

        public TestBlock(ColorEnum color)
        {
            Color = color;
        }
    }
}