using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Inherit
{
    public class ParentTrait : BaseBlock
    {
        [PropertyState("color")]
        public enum ColorEnum
        {
            [PropertyCondition("red")] Red,
            [PropertyCondition("green")] Green,
            [PropertyCondition("blue")] Blue
        }
    }
}