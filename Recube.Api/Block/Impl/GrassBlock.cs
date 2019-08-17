namespace Recube.Api.Block.Impl
{
	[Block("minecraft:grass_block", typeof(SnowyProperty))]
	public class GrassBlock : BaseBlock
	{
		[PropertyState("snowy")]
		public enum SnowyProperty
		{
			[PropertyCondition(false)] Default,
			[PropertyCondition(true)] Snowy
		}

		public GrassBlock(SnowyProperty snowy)
		{
			Snowy = snowy;
		}

		public SnowyProperty Snowy { get; }
	}
}