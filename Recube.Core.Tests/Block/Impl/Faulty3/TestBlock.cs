using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Faulty3
{
    [Block("recube:testblock")]
    public class TestBlock : BaseBlock
    {
        [PropertyState("color")]
        public enum ColorEnum
        {
            [PropertyCondition("red")] Red,
            [PropertyCondition("red")] Green,
            [PropertyCondition("blue")] Blue
        }
    }
}