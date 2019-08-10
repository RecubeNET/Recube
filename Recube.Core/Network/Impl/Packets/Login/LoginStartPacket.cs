using System;
using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Start
{
	[Packet(0x0, NetworkPlayerState.Login)]
	// ReSharper disable once UnusedMember.Global
	public class LoginStartPacket : IInPacket
	{
		public String username;
		public void Read(IByteBuffer buffer)
		{
			username = buffer.ReadStringWithLength();
		}
	}
}