using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Faulty4
{
    [Block("recube:testblock")]
    public class TestBlock : BaseBlock
    {
        public ColorEnum Color { get; set; }

        [PropertyState("color")]
        public enum ColorEnum
        {
            [PropertyCondition("red")] Red,
            [PropertyCondition("red")] Green,
            [PropertyCondition("blue")] Blue
        }
    }
}