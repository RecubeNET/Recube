using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Pipeline
{
	public class PacketEncoder : MessageToByteEncoder<IOutPacket>
	{
		protected override void Encode(IChannelHandlerContext context, IOutPacket message, IByteBuffer output)
		{
			var player = Recube.Instance.NetworkPlayerRegistry.GetNetworkPlayerByChannel(context.Channel);
			if (player == null) throw new InvalidOperationException("NetworkPlayer is null");

			var packetRegistry =
				Recube.Instance.GetCorrectPacketRegistry(player.CurrentState, PacketDirection.Outbound);

			var nullablePacketId = packetRegistry.GetPacketId(message);
			if (!nullablePacketId.HasValue)
			{
				NetworkBootstrap.Logger.Warn(
					$"Tried to send a packet which is not registered (in the current state: ${Enum.GetName(typeof(NetworkPlayerState), player.CurrentState)})");
				return;
			}

			var packetId = nullablePacketId.Value;

			output.WriteVarInt(packetId);
			message.Write(output);
		}
	}
}