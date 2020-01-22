using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Util;

namespace Recube.Api.Network.Impl.Packets.Login
{
	[Packet(0x02, NetworkPlayerState.Login)]
	public class LoginSuccessOutPacket : IOutPacket
	{
		public string Username;
		public Uuid Uuid;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteStringWithLength(Uuid.ToString());
			buffer.WriteStringWithLength(Username);
		}
	}
}