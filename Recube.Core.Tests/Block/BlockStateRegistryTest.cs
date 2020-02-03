using System.Collections.Generic;
using Recube.Core.Block;
using Recube.Core.Tests.Block.Impl.Correct;
using Xunit;

namespace Recube.Core.Tests.Block
{
    public class BlockStateRegistryTest
    {
        private readonly BlockStateRegistry _blockStateRegistry;

        public BlockStateRegistryTest()
        {
            var fileBlocks = BlockParser.ParseFile("Block/test_blocks.json").GetAwaiter().GetResult();
            var blockClasses = BlockParser.ParseBlockClasses("Recube.Core.Tests.Block.Impl.Correct");

            _blockStateRegistry = new BlockStateRegistry(fileBlocks, blockClasses);
        }

        // @formatter:off
        [Fact]
        public void FromRawTest()
        {
            //recube:testblock
            Assert.Equal(0,_blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "green"}}).NetworkId);
            Assert.Equal(1,_blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "blue"}}).NetworkId);
            Assert.Equal(2,_blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "red"}}).NetworkId);
            
            // CHECK FOR DEFAULT
            Assert.Equal(2,_blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"color", "2"}}).NetworkId); 
            Assert.Equal(2,_blockStateRegistry.FromRaw("recube:testblock", new Dictionary<string, string> {{"collor", "green"}}).NetworkId);
            
            
            //recube:testblock2
            Assert.Equal(3,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "green"}, {"shape", "triangle"}}).NetworkId);
            Assert.Equal(4,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "blue"}, {"shape", "triangle"}}).NetworkId);
            Assert.Equal(5,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"shape", "triangle"}}).NetworkId);
            Assert.Equal(6,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "green"}, {"shape", "cube"}}).NetworkId);
            Assert.Equal(7,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "blue"}, {"shape", "cube"}}).NetworkId);
            Assert.Equal(8,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"shape", "cube"}}).NetworkId);
            Assert.Equal(9,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "green"}, {"shape", "sphere"}}).NetworkId);
            Assert.Equal(10,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "blue"}, {"shape", "sphere"}}).NetworkId);
            Assert.Equal(11,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"shape", "sphere"}}).NetworkId);
            
            //CHECK FOR DEFAULT
            Assert.Equal(3,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string>()).NetworkId);
            Assert.Equal(3,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"abc", "a"}}).NetworkId);
            Assert.Equal(3,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}}).NetworkId);
            Assert.Equal(3,_blockStateRegistry.FromRaw("recube:testblock2", new Dictionary<string, string> {{"color", "red"}, {"",""}}).NetworkId);
        }

        [Fact]
        public void GetBaseBlockFromStateTest()
        {
            Assert.Null(_blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(200)));
            Assert.Null( _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(3)) as TestBlock);
            
  
            
            Assert.Equal(TestBlock.ColorEnum.Green, ((TestBlock) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(0))).Color);
            Assert.Equal(TestBlock.ColorEnum.Blue, ((TestBlock) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(1))).Color);
            Assert.Equal(TestBlock.ColorEnum.Red, ((TestBlock) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(2))).Color);
            
            
            
            Assert.Equal(TestBlock2.ColorEnum.Green, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(3))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Triangle, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(3))).Shape);
            
            Assert.Equal(TestBlock2.ColorEnum.Blue, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(4))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Triangle, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(4))).Shape);
             
            Assert.Equal(TestBlock2.ColorEnum.Red, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(5))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Triangle, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(5))).Shape);
            
            
            Assert.Equal(TestBlock2.ColorEnum.Green, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(6))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Cube, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(6))).Shape);
            
            Assert.Equal(TestBlock2.ColorEnum.Blue, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(7))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Cube, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(7))).Shape);
             
            Assert.Equal(TestBlock2.ColorEnum.Red, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(8))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Cube, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(8))).Shape);
            
            
            Assert.Equal(TestBlock2.ColorEnum.Green, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(9))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Sphere, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(9))).Shape);
            
            Assert.Equal(TestBlock2.ColorEnum.Blue, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(10))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Sphere, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(10))).Shape);
             
            Assert.Equal(TestBlock2.ColorEnum.Red, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(11))).Color);
            Assert.Equal(TestBlock2.ShapeEnum.Sphere, ((TestBlock2) _blockStateRegistry.GetBaseBlockFromState(_blockStateRegistry.GetBlockStateByNetworkId(11))).Shape);
        }
        
        [Fact]
        public void GetStateByBaseBlockTest() 
        {
            Assert.Equal(0, _blockStateRegistry.GetStateByBaseBlock(new TestBlock(TestBlock.ColorEnum.Green)).NetworkId);
            Assert.Equal(1, _blockStateRegistry.GetStateByBaseBlock(new TestBlock(TestBlock.ColorEnum.Blue)).NetworkId);
            Assert.Equal(2, _blockStateRegistry.GetStateByBaseBlock(new TestBlock(TestBlock.ColorEnum.Red)).NetworkId);
            
            
            Assert.Equal(3, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Green, TestBlock2.ShapeEnum.Triangle)).NetworkId);
            Assert.Equal(4, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Blue, TestBlock2.ShapeEnum.Triangle)).NetworkId);
            Assert.Equal(5, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Red, TestBlock2.ShapeEnum.Triangle)).NetworkId);
            
            Assert.Equal(6, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Green, TestBlock2.ShapeEnum.Cube)).NetworkId);
            Assert.Equal(7, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Blue, TestBlock2.ShapeEnum.Cube)).NetworkId);
            Assert.Equal(8, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Red, TestBlock2.ShapeEnum.Cube)).NetworkId);
            
            Assert.Equal(9, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Green, TestBlock2.ShapeEnum.Sphere)).NetworkId);
            Assert.Equal(10, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Blue, TestBlock2.ShapeEnum.Sphere)).NetworkId);
            Assert.Equal(11, _blockStateRegistry.GetStateByBaseBlock(new TestBlock2(TestBlock2.ColorEnum.Red, TestBlock2.ShapeEnum.Sphere)).NetworkId);
        }
        
        // @formatter:on
        [Fact]
        public void GetBlockStateByNetworkIdTest()
        {
        }
    }
}