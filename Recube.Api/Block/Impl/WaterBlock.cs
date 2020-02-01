namespace Recube.Api.Block.Impl
{
    [Block("minecraft:water")]
    public class WaterBlock : BaseBlock
    {
        public LevelProperty Level { get; set; }

        public WaterBlock(LevelProperty level)
        {
            Level = level;
        }

        [PropertyState("level")]
        public enum LevelProperty
        {
            [PropertyCondition("0")] Zero,
            [PropertyCondition("1")] One,
            [PropertyCondition("2")] Two,
            [PropertyCondition("3")] Three,
            [PropertyCondition("4")] Four,
            [PropertyCondition("5")] Five,
            [PropertyCondition("6")] Six,
            [PropertyCondition("7")] Seven,
            [PropertyCondition("8")] Eight,
            [PropertyCondition("9")] Nine,
            [PropertyCondition("10")] Ten,
            [PropertyCondition("11")] Eleven,
            [PropertyCondition("12")] Twelve,
            [PropertyCondition("13")] Thirteen,
            [PropertyCondition("14")] Fourteen,
            [PropertyCondition("15")] Fifteen
        }
    }
}