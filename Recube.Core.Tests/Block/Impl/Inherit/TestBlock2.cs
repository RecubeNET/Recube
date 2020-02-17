using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Inherit
{
    [Block("recube:testblock2")]
    public class TestBlock2 : ParentTrait
    {
        public ColorEnum Color { get; set; }
        public ShapeEnum Shape { get; set; }

        public TestBlock2(ColorEnum color, ShapeEnum shape)
        {
            Color = color;
            Shape = shape;
        }

        [PropertyState("shape")]
        public enum ShapeEnum
        {
            [PropertyCondition("triangle")] Triangle,
            [PropertyCondition("cube")] Cube,
            [PropertyCondition("sphere")] Sphere
        }
    }
}