using DotNetty.Buffers;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x49, NetworkPlayerState.Play)]
	public class SpawnPositionOutPacket : IOutPacket
	{
		private long _position;
		public int X;
		public int Y;
		public int Z;

		public void Write(IByteBuffer buffer)
		{
			_position = ((long) X & 0x3FFFFFF) << 38 | ((long) Y & 0xFFF) << 26 | (long) Z & 0x3FFFFFF;
			buffer.WriteLong(_position);
		}
	}
}