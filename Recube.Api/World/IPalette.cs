using DotNetty.Buffers;
using Recube.Api.Block;

namespace Recube.Api.World
{
	public interface IPalette
	{
		uint IdForState(BlockState state);
		BlockState StateForId(uint id);
		byte GetBitsPerBlock();
		void Read(IByteBuffer data);
		void Write(IByteBuffer data);
	}
}