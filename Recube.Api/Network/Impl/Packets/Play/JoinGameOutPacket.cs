using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x26, NetworkPlayerState.Play)]
	public class JoinGameOutPacket : IOutPacket
	{
		public int Dimension;
		public bool EnableRespawnScreen;
		public int EntityId;
		public byte Gamemode;
		public long HashedSeed;
		public string LevelType;
		public byte MaxPlayers;
		public bool ReducedDebugInfo;
		public int ViewDistance;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteInt(EntityId);
			buffer.WriteByte(Gamemode);
			buffer.WriteInt(Dimension);
			buffer.WriteLong(HashedSeed);
			buffer.WriteByte(MaxPlayers);
			buffer.WriteStringWithLength(LevelType);
			buffer.WriteVarInt(ViewDistance);
			buffer.WriteBoolean(ReducedDebugInfo);
			buffer.WriteBoolean(EnableRespawnScreen);
		}
	}
}