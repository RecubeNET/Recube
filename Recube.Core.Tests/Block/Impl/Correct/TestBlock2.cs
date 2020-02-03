using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Correct
{
    [Block("recube:testblock2")]
    public class TestBlock2 : BaseBlock
    {
        public ColorEnum Color { get; set; }
        public ShapeEnum Shape { get; set; }

        public TestBlock2(ColorEnum color, ShapeEnum shape)
        {
            Color = color;
            Shape = shape;
        }

        [PropertyState("color")]
        public enum ColorEnum
        {
            [PropertyCondition("red")] Red,
            [PropertyCondition("green")] Green,
            [PropertyCondition("blue")] Blue
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