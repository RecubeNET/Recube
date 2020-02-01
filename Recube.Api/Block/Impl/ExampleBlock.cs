namespace Recube.Api.Block.Impl
{
    /// <summary>
    /// This is an example block to demonstrate how a block class has to look like.
    /// </summary>
    // First: Add the attribute containing the block name as stated in blocks.json
    [Block("recube:example_block")]
    // This is optional and ensures that this example block will not be parsed.
    [NoParse]
    public class ExampleBlock : BaseBlock // Inherit BaseBlock
    {
        // There is exactly one property stated in blocks.json and this is the field for it.
        public ColorProperty Color { get; set; }

        // We have one property to fulfill so this constructor has only the parameter "ColorProperty"
        public ExampleBlock(ColorProperty color)
        {
            Color = color;
        }

        // This attribute is necessary so that our parser knows which field you're targeting
        [PropertyState("color")]
        public enum ColorProperty
        {
            // If color equals to "red"
            [PropertyCondition("red")] Red,

            // If color equals to "green"
            [PropertyCondition("green")] Green,

            // etc.
            [PropertyCondition("blue")] Blue,
            [PropertyCondition("white")] White,
            [PropertyCondition("black")] Black,
            [PropertyCondition("transparent")] Transparent
        }
    }
}