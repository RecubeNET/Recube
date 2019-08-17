using DotNetty.Buffers;
using Recube.Api.Network.Entities;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Login
{
	[Packet(0x02, NetworkPlayerState.Login)]
	public class LoginSuccessPacket : IOutPacket
	{
		public string Username;
		public UUID Uuid;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteStringWithLength(Uuid.ToString());
			buffer.WriteStringWithLength(Username);
		}
	}
}