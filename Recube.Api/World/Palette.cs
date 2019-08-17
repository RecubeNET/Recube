using DotNetty.Buffers;
using Recube.Api.Block;

namespace Recube.Core.World
{
	public interface Palette
	{
		uint IdForState(BlockState state);
		BlockState StateForId(uint id);
		byte GetBitsPerBlock();
		void Read(IByteBuffer data);
		void Write(IByteBuffer data);
	}
}