using Recube.Core.Block;
using Xunit;

namespace Recube.Core.Tests.Block
{
    public class BlockParserTest
    {
        #region ClassParserTest

        [Fact]
        public void TestSuccessfulParse()
        {
            var blocks = BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Correct");
            Assert.Equal(2, blocks.Count);
        }


        /// <summary>
        /// Check if a <see cref="BlockParseException"/> is thrown when no property-matching constructor could be found
        /// </summary>
        [Fact]
        public void CheckFaulty1()
        {
            Assert.Throws<BlockParseException>(() =>
                BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Faulty"));
        }

        /// <summary>
        /// Check if a <see cref="PropertyParseException"/> is thrown when a property is missing a PropertyCondition attribute
        /// </summary>
        [Fact]
        public void CheckFaulty2()
        {
            Assert.Throws<PropertyParseException>(() =>
                BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Faulty2"));
        }

        /// <summary>
        /// Check if a <see cref="PropertyParseException"/> is thrown when a property has no designated field in the declared type
        /// </summary>
        [Fact]
        public void CheckFaulty3()
        {
            Assert.Throws<PropertyParseException>(() =>
                BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Faulty2"));
        }

        /// <summary>
        /// Check if a <see cref="BlockParseException"/> is thrown when a property field is declared but not set in the constructor
        /// </summary>
        [Fact]
        public void CheckFaulty4()
        {
            Assert.Throws<BlockParseException>(() =>
                BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Faulty4"));
        }

        #endregion

        #region FileParseTest

        [Fact]
        public void FileParseTest()
        {
            var parsed = BlockParser.ParseFile("Block/test_blocks.json").GetAwaiter().GetResult();

            var testBlock1 = parsed["recube:testblock"];
            var testBlock2 = parsed["recube:testblock2"];

            {
                Assert.Equal(3, testBlock1.Count);
                var state1 = testBlock1[0];
                var state2 = testBlock1[1];
                var state3 = testBlock1[2];
                Assert.Equal(0, state1.NetworkId);
                Assert.Single(state1.Properties);
                Assert.Equal("green", state1.Properties["color"]);
                Assert.False(state1.Default);
                //
                Assert.Equal(1, state2.NetworkId);
                Assert.Single(state2.Properties);
                Assert.Equal("blue", state2.Properties["color"]);
                Assert.False(state2.Default);
                //
                Assert.Equal(2, state3.NetworkId);
                Assert.Single(state3.Properties);
                Assert.Equal("red", state3.Properties["color"]);
                Assert.True(state3.Default);
            }
            {
                Assert.Equal(9, testBlock2.Count);
                var state1 = testBlock2[0];
                var state2 = testBlock2[1];
                var state3 = testBlock2[2];
                var state4 = testBlock2[3];
                var state5 = testBlock2[4];
                var state6 = testBlock2[5];
                var state7 = testBlock2[6];
                var state8 = testBlock2[7];
                var state9 = testBlock2[8];

                Assert.Equal(3, state1.NetworkId);
                Assert.Equal(2, state1.Properties.Count);
                Assert.Equal("green", state1.Properties["color"]);
                Assert.Equal("triangle", state1.Properties["shape"]);
                Assert.True(state1.Default);
                //
                Assert.Equal(4, state2.NetworkId);
                Assert.Equal(2, state2.Properties.Count);
                Assert.Equal("blue", state2.Properties["color"]);
                Assert.Equal("triangle", state2.Properties["shape"]);
                Assert.False(state2.Default);
                //
                Assert.Equal(5, state3.NetworkId);
                Assert.Equal(2, state3.Properties.Count);
                Assert.Equal("red", state3.Properties["color"]);
                Assert.Equal("triangle", state3.Properties["shape"]);
                Assert.False(state3.Default);
                //
                Assert.Equal(6, state4.NetworkId);
                Assert.Equal(2, state4.Properties.Count);
                Assert.Equal("green", state4.Properties["color"]);
                Assert.Equal("cube", state4.Properties["shape"]);
                Assert.False(state4.Default);
                //
                Assert.Equal(7, state5.NetworkId);
                Assert.Equal(2, state5.Properties.Count);
                Assert.Equal("blue", state5.Properties["color"]);
                Assert.Equal("cube", state5.Properties["shape"]);
                Assert.False(state5.Default);
                //
                Assert.Equal(8, state6.NetworkId);
                Assert.Equal(2, state6.Properties.Count);
                Assert.Equal("red", state6.Properties["color"]);
                Assert.Equal("cube", state6.Properties["shape"]);
                Assert.False(state6.Default);
                //
                Assert.Equal(9, state7.NetworkId);
                Assert.Equal(2, state7.Properties.Count);
                Assert.Equal("green", state7.Properties["color"]);
                Assert.Equal("sphere", state7.Properties["shape"]);
                Assert.False(state7.Default);
                //
                Assert.Equal(10, state8.NetworkId);
                Assert.Equal(2, state8.Properties.Count);
                Assert.Equal("blue", state8.Properties["color"]);
                Assert.Equal("sphere", state8.Properties["shape"]);
                Assert.False(state8.Default);
                //
                Assert.Equal(11, state9.NetworkId);
                Assert.Equal(2, state9.Properties.Count);
                Assert.Equal("red", state9.Properties["color"]);
                Assert.Equal("sphere", state9.Properties["shape"]);
                Assert.False(state9.Default);
            }
        }

        #endregion
    }
}