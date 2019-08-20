using DotNetty.Buffers;
using Recube.Api.Entities.DataStructures;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x49, NetworkPlayerState.Play)]
	public class SpawnPositionOutPacket : IOutPacket
	{
		public BlockPosition BlockPosition;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteLong(BlockPosition.ToLong());
		}
	}
}