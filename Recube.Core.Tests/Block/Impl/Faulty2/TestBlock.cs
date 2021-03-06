﻿using Recube.Api.Block;

namespace Recube.Core.Tests.Block.Impl.Faulty2
{
    [Block("recube:testblock")]
    public class TestBlock : BaseBlock
    {
        public ColorEnum Color { get; set; }

        public TestBlock(ColorEnum color)
        {
            Color = color;
        }

        [PropertyState("color")]
        public enum ColorEnum
        {
            [PropertyCondition("red")] Red,
            Green,
            [PropertyCondition("blue")] Blue
        }
    }
}