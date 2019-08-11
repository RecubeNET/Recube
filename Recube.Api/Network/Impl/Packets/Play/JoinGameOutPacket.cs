using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	[Packet(0x25, NetworkPlayerState.Play)]
	public class JoinGameOutPacket : IOutPacket
	{
		public byte Difficulty;
		public int Dimension;
		public int EntityId;
		public byte Gamemode;
		public string LevelType;
		public byte MaxPlayers;
		public bool ReducedDebugInfo;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteInt(EntityId);
			buffer.WriteByte(Gamemode);
			buffer.WriteInt(Dimension);
			buffer.WriteByte(Difficulty);
			buffer.WriteByte(MaxPlayers);
			buffer.WriteStringWithLength(LevelType);
			buffer.WriteBoolean(ReducedDebugInfo);
		}
	}
}