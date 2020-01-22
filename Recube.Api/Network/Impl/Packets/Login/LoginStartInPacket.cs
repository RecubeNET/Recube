using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Login
{
	[Packet(0x0, NetworkPlayerState.Login)]
	public class LoginStartInPacket : IInPacket
	{
		public string Username;

		public void Read(IByteBuffer buffer)
		{
			Username = buffer.ReadStringWithLength();
		}
	}
}