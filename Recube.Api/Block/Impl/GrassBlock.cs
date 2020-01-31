namespace Recube.Api.Block.Impl
{
    [Block("minecraft:grass_block")]
    public class GrassBlock : BaseBlock
    {
        public SnowyProperty Snowy { get; }

        public GrassBlock(SnowyProperty snowy)
        {
            Snowy = snowy;
        }


        [PropertyState("snowy")]
        public enum SnowyProperty
        {
            [PropertyCondition(false)] Default,
            [PropertyCondition(true)] Snowy
        }
    }
}