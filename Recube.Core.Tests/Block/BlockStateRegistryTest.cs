using System.Collections.Generic;
using Recube.Api.Block;
using Recube.Core.Block;
using Xunit;

namespace Recube.Core.Tests.Block
{
    public class BlockStateRegistryTest
    {
        // @formatter:off
        public static Dictionary<string, List<BlockState>> _fileBlocks = BlockParser.ParseFile("Block/test_blocks.json").GetAwaiter().GetResult();
        public static BlockStateRegistry _correctRegistry = new BlockStateRegistry(_fileBlocks, BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Correct"));
        // InerhitRegistry tests the same things as the CorrectRegistry but with inheritance
        public static BlockStateRegistry _inheritRegistry = new BlockStateRegistry(_fileBlocks, BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Inherit"));

        public static IEnumerable<object[]> _toTestRegistries() => new List<object[]>
        {
            new object[] {_correctRegistry},
            new object[] {_inheritRegistry},
        };

        #region Theory tests
        [Theory]
        [MemberData(nameof(_toTestRegistries))]
        public void FromRawTest(BlockStateRegistry blockStateRegistry)
        {
            //recube:testblock
            Assert.Equal(0,blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "green"}}).NetworkId);
            Assert.Equal(1,blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "blue"}}).NetworkId);
            Assert.Equal(2,blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "red"}}).NetworkId);
            
            // CHECK FOR DEFAULT
            Assert.Equal(2,blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "2"}}).NetworkId); 
            Assert.Equal(2,blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"collor", "green"}}).NetworkId);
            
            
            //recube:testblock2
            Assert.Equal(3,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "green"}, {"shape", "triangle"}}).NetworkId);
            Assert.Equal(4,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "blue"}, {"shape", "triangle"}}).NetworkId);
            Assert.Equal(5,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"shape", "triangle"}}).NetworkId);
            Assert.Equal(6,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "green"}, {"shape", "cube"}}).NetworkId);
            Assert.Equal(7,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "blue"}, {"shape", "cube"}}).NetworkId);
            Assert.Equal(8,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"shape", "cube"}}).NetworkId);
            Assert.Equal(9,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "green"}, {"shape", "sphere"}}).NetworkId);
            Assert.Equal(10,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "blue"}, {"shape", "sphere"}}).NetworkId);
            Assert.Equal(11,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"shape", "sphere"}}).NetworkId);
            
            //CHECK FOR DEFAULT
            Assert.Equal(3,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string>()).NetworkId);
            Assert.Equal(3,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"abc", "a"}}).NetworkId);
            Assert.Equal(3,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}}).NetworkId);
            Assert.Equal(3,blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"",""}}).NetworkId);
        }

        [Theory]
        [MemberData(nameof(_toTestRegistries))]
        public void GetBlockStateByNetworkIdTest(BlockStateRegistry blockStateRegistry)
        {
            Assert.Equal(new BlockState("recube:testblock", 0, false, new Dictionary<string, string> {{"color", "green"}}),blockStateRegistry.GetBlockStateByNetworkId(0));
            Assert.Equal(new BlockState("recube:testblock", 1, false, new Dictionary<string, string> {{"color", "blue"}}),blockStateRegistry.GetBlockStateByNetworkId(1));
            Assert.Equal(new BlockState("recube:testblock", 2, true, new Dictionary<string, string> {{"color", "red"}}),blockStateRegistry.GetBlockStateByNetworkId(2));
           
            
            Assert.Equal(new BlockState("recube:testblock2", 3, true, new Dictionary<string, string> {{"color", "green"}, {"shape", "triangle"}}),blockStateRegistry.GetBlockStateByNetworkId(3));
            Assert.Equal(new BlockState("recube:testblock2", 4, false, new Dictionary<string, string> {{"color", "blue"}, {"shape", "triangle"}}),blockStateRegistry.GetBlockStateByNetworkId(4));
            Assert.Equal(new BlockState("recube:testblock2", 5, false, new Dictionary<string, string> {{"color", "red"}, {"shape", "triangle"}}),blockStateRegistry.GetBlockStateByNetworkId(5));
           
            Assert.Equal(new BlockState("recube:testblock2", 6, false, new Dictionary<string, string> {{"color", "green"}, {"shape", "cube"}}),blockStateRegistry.GetBlockStateByNetworkId(6));
            Assert.Equal(new BlockState("recube:testblock2", 7, false, new Dictionary<string, string> {{"color", "blue"}, {"shape", "cube"}}),blockStateRegistry.GetBlockStateByNetworkId(7));
            Assert.Equal(new BlockState("recube:testblock2", 8, false, new Dictionary<string, string> {{"color", "red"}, {"shape", "cube"}}),blockStateRegistry.GetBlockStateByNetworkId(8));
            
            Assert.Equal(new BlockState("recube:testblock2", 9, false, new Dictionary<string, string> {{"color", "green"}, {"shape", "sphere"}}),blockStateRegistry.GetBlockStateByNetworkId(9));
            Assert.Equal(new BlockState("recube:testblock2", 10, false, new Dictionary<string, string> {{"color", "blue"}, {"shape", "sphere"}}),blockStateRegistry.GetBlockStateByNetworkId(10));
            Assert.Equal(new BlockState("recube:testblock2", 11, false, new Dictionary<string, string> {{"color", "red"}, {"shape", "sphere"}}),blockStateRegistry.GetBlockStateByNetworkId(11));
        }
        #endregion
        
        
        #region CorrectRegistry tests
        [Fact]
        public void GetBaseBlockFromStateTestWithCorrectRegistry()
        {
            var blockStateRegistry = _correctRegistry;  
            
            Assert.Null(blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(200)));
            Assert.Null( blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(3)) as Impl.Correct.TestBlock);
            
  
            
            Assert.Equal(Impl.Correct.TestBlock.ColorEnum.Green, ((Impl.Correct.TestBlock) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(0))).Color);
            Assert.Equal(Impl.Correct.TestBlock.ColorEnum.Blue, ((Impl.Correct.TestBlock) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(1))).Color);
            Assert.Equal(Impl.Correct.TestBlock.ColorEnum.Red, ((Impl.Correct.TestBlock) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(2))).Color);
            
            
            
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Green, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(3))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Triangle, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(3))).Shape);
            
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Blue, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(4))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Triangle, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(4))).Shape);
             
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Red, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(5))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Triangle, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(5))).Shape);
            
            
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Green, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(6))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Cube, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(6))).Shape);
            
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Blue, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(7))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Cube, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(7))).Shape);
             
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Red, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(8))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Cube, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(8))).Shape);
            
            
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Green, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(9))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Sphere, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(9))).Shape);
            
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Blue, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(10))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Sphere, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(10))).Shape);
             
            Assert.Equal(Impl.Correct.TestBlock2.ColorEnum.Red, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(11))).Color);
            Assert.Equal(Impl.Correct.TestBlock2.ShapeEnum.Sphere, ((Impl.Correct.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(11))).Shape);
        }

        [Fact]
        public void GetStateByBaseBlockTestWithCorrectRegistry() 
        {
            var blockStateRegistry = _correctRegistry;
            Assert.Equal(0, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock(Impl.Correct.TestBlock.ColorEnum.Green)).NetworkId);
            Assert.Equal(1, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock(Impl.Correct.TestBlock.ColorEnum.Blue)).NetworkId);
            Assert.Equal(2, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock(Impl.Correct.TestBlock.ColorEnum.Red)).NetworkId);
            
            
            Assert.Equal(3, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Green, Impl.Correct.TestBlock2.ShapeEnum.Triangle)).NetworkId);
            Assert.Equal(4, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Blue, Impl.Correct.TestBlock2.ShapeEnum.Triangle)).NetworkId);
            Assert.Equal(5, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Red, Impl.Correct.TestBlock2.ShapeEnum.Triangle)).NetworkId);
            
            Assert.Equal(6, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Green, Impl.Correct.TestBlock2.ShapeEnum.Cube)).NetworkId);
            Assert.Equal(7, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Blue, Impl.Correct.TestBlock2.ShapeEnum.Cube)).NetworkId);
            Assert.Equal(8, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Red, Impl.Correct.TestBlock2.ShapeEnum.Cube)).NetworkId);
            
            Assert.Equal(9, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Green, Impl.Correct.TestBlock2.ShapeEnum.Sphere)).NetworkId);
            Assert.Equal(10, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Blue, Impl.Correct.TestBlock2.ShapeEnum.Sphere)).NetworkId);
            Assert.Equal(11, blockStateRegistry.GetStateByBaseBlock(new Impl.Correct.TestBlock2(Impl.Correct.TestBlock2.ColorEnum.Red, Impl.Correct.TestBlock2.ShapeEnum.Sphere)).NetworkId);
        }
        #endregion
        
        #region InheritRegistry tests
        [Fact]
        public void GetBaseBlockFromStateTestWithInheritRegistry()
        {
            var blockStateRegistry = _inheritRegistry;  
            
            Assert.Null(blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(200)));
            Assert.Null( blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(3)) as Impl.Inherit.TestBlock);
            
  
            
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Green, ((Impl.Inherit.TestBlock) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(0))).Color);
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Blue, ((Impl.Inherit.TestBlock) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(1))).Color);
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Red, ((Impl.Inherit.TestBlock) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(2))).Color);
            
            
            
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Green, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(3))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Triangle, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(3))).Shape);
            
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Blue, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(4))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Triangle, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(4))).Shape);
             
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Red, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(5))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Triangle, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(5))).Shape);
            
            
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Green, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(6))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Cube, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(6))).Shape);
            
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Blue, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(7))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Cube, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(7))).Shape);
             
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Red, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(8))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Cube, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(8))).Shape);
            
            
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Green, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(9))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Sphere, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(9))).Shape);
            
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Blue, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(10))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Sphere, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(10))).Shape);
             
            Assert.Equal(Impl.Inherit.ParentTrait.ColorEnum.Red, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(11))).Color);
            Assert.Equal(Impl.Inherit.TestBlock2.ShapeEnum.Sphere, ((Impl.Inherit.TestBlock2) blockStateRegistry.GetBaseBlockFromState(blockStateRegistry.GetBlockStateByNetworkId(11))).Shape);
        }

        [Fact]
        public void GetStateByBaseBlockTestWithInheritRegistry() 
        {
            var blockStateRegistry = _inheritRegistry;
            Assert.Equal(0, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock(Impl.Inherit.ParentTrait.ColorEnum.Green)).NetworkId);
            Assert.Equal(1, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock(Impl.Inherit.ParentTrait.ColorEnum.Blue)).NetworkId);
            Assert.Equal(2, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock(Impl.Inherit.ParentTrait.ColorEnum.Red)).NetworkId);
            
            
            Assert.Equal(3, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Green, Impl.Inherit.TestBlock2.ShapeEnum.Triangle)).NetworkId);
            Assert.Equal(4, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Blue, Impl.Inherit.TestBlock2.ShapeEnum.Triangle)).NetworkId);
            Assert.Equal(5, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Red, Impl.Inherit.TestBlock2.ShapeEnum.Triangle)).NetworkId);
            
            Assert.Equal(6, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Green, Impl.Inherit.TestBlock2.ShapeEnum.Cube)).NetworkId);
            Assert.Equal(7, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Blue, Impl.Inherit.TestBlock2.ShapeEnum.Cube)).NetworkId);
            Assert.Equal(8, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Red, Impl.Inherit.TestBlock2.ShapeEnum.Cube)).NetworkId);
            
            Assert.Equal(9, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Green, Impl.Inherit.TestBlock2.ShapeEnum.Sphere)).NetworkId);
            Assert.Equal(10, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Blue, Impl.Inherit.TestBlock2.ShapeEnum.Sphere)).NetworkId);
            Assert.Equal(11, blockStateRegistry.GetStateByBaseBlock(new Impl.Inherit.TestBlock2(Impl.Inherit.ParentTrait.ColorEnum.Red, Impl.Inherit.TestBlock2.ShapeEnum.Sphere)).NetworkId);
        }
        #endregion
    }
}