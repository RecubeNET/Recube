using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Recube.Api.Network.Packets;
using Recube.Core.Network.Packets.Handler;

namespace Recube.Core.Network.Packets
{
	public class PacketInboundHandler : SimpleChannelInboundHandler<IPacket>
	{
		private NetworkPlayer.NetworkPlayer _player;

		protected override void ChannelRead0(IChannelHandlerContext ctx, IPacket msg)
		{
			_player.PacketHandler.FirePacket(msg as IInPacket);
		}

		public override void ChannelRegistered(IChannelHandlerContext context)
		{
			_player = new NetworkPlayer.NetworkPlayer(context.Channel);
			Recube.Instance.NetworkPlayerRegistry.Register(_player);
			_player.SetPacketHandler(
				PacketHandlerFactory.CreatePacketHandler(Recube.Instance.HandshakePacketHandler, _player));
		}

		public override void ChannelUnregistered(IChannelHandlerContext context)
		{
			Recube.Instance.NetworkPlayerRegistry.Deregister(_player);
		}

		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			NetworkBootstrap.Logger.Error("Exception caught in channel {}! Closing channel: \n{}", _player.Channel,
				exception);
			Task.Run(() => _player.DisconnectAsync());
		}
	}
}