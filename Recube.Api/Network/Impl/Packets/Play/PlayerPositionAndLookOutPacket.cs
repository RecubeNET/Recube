using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x32, NetworkPlayerState.Play)]
	public class PlayerPositionAndLookOutPacket : IOutPacket
	{
		public byte Flags;
		public float Pitch;
		public int TeleportId;
		public double X;
		public double Y;
		public float Yaw;
		public double Z;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
			buffer.WriteFloat(Yaw);
			buffer.WriteFloat(Pitch);
			buffer.WriteByte(Flags);
			buffer.WriteVarInt(TeleportId);
		}
	}
}