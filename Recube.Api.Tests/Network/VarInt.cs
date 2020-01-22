using System;
using DotNetty.Buffers;
using Xunit;

namespace Recube.Api.Tests.Network
{
	public class VarInt
	{
		[Fact]
		public void WriteVarIntTest1B()
		{
			Api.Network.VarInt.WriteVarInt(0x20, out var bytes);
			Assert.Equal( new byte[] { 32 },bytes);
		}
		
		[Fact]
		public void WriteVarIntTest2B()
		{
			Api.Network.VarInt.WriteVarInt(5000, out var bytes);
			Assert.Equal( new byte[] { 136, 39 },bytes);
		}

		[Fact]
		public void WriteVarIntTest5B()
		{
			Api.Network.VarInt.WriteVarInt(268435456, out var bytes);
			Assert.Equal( new byte[] { 128, 128, 128, 128, 1 },bytes);
		}

		
		
		[Fact]
		public void WriteVarLongTest5B()
		{
			Api.Network.VarInt.WriteVarLong(268435456, out var bytes);
			Assert.Equal( new byte[] { 128, 128, 128, 128, 1 },bytes);
		}
		
		[Fact]
		public void ReadVarIntTest1B()
		{
			var buf = Unpooled.WrappedBuffer(new byte[] {32});
			Api.Network.VarInt.ReadVarInt(buf, out var result);
			Assert.NotNull(result);
			Assert.Equal(0x20, result.Value);
		}

		
		[Fact]
		public void ReadVarIntTest2B()
		{
			var buf = Unpooled.WrappedBuffer(new byte[] {136, 39});
			Api.Network.VarInt.ReadVarInt(buf, out var result);
			Assert.NotNull(result);
			Assert.Equal(5000, result.Value);
		}
		
		[Fact]
		public void ReadVarIntTest5B()
		{
			var buf = Unpooled.WrappedBuffer(new byte[] { 128, 128, 128, 128, 1});
			Api.Network.VarInt.ReadVarInt(buf, out var result);
			Assert.NotNull(result);
			Assert.Equal(268435456, result.Value);
		}
		
		
		[Fact]
		public void ReadVarLongTest5B()
		{
			var buf = Unpooled.WrappedBuffer(new byte[] { 128, 128, 128, 128, 1});
			Api.Network.VarInt.ReadVarLong(buf, out var result);
			Assert.NotNull(result);
			Assert.Equal(268435456, result.Value);
		}
	}
}