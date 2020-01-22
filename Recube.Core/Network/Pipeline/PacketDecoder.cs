using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Recube.Api.Network;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Pipeline
{
	public class PacketDecoder : ByteToMessageDecoder
	{
		/// Be careful: Input is a stream, so the ReaderIndex/WriterIndex won't be 0 on for example packet nr. 2 
		protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
		{
			try
			{
				if (!VarInt.ReadVarInt(input, out var nullablePacketId) || !nullablePacketId.HasValue)
				{
					NetworkBootstrap.Logger.Warn("Received packet with invalid packet id");
					return;
				}

				var packetId = nullablePacketId.Value;

				var player = Recube.Instance.NetworkPlayerRegistry.GetNetworkPlayerByChannel(context.Channel);
				if (player == null) throw new InvalidOperationException("NetworkPlayer is null");

				var packetRegistry = Recube.Instance.GetCorrectPacketRegistry(player.CurrentState, PacketDirection.Inbound);

				if (!(packetRegistry.GetPacketById(packetId) is IInPacket packet))
				{
					NetworkBootstrap.Logger.Warn(
						$"Received packet 0x{packetId:X}[{packetId}] which is not registered (in the current state:  {Enum.GetName(typeof(NetworkPlayerState), player.CurrentState)})");
					return;
				}

				NetworkBootstrap.Logger.Debug($"Received packet {packet.GetType().FullName}");
				try
				{
					packet.Read(input);
				}
				catch (Exception e)
				{
					throw new Exception(
						$"Could not call packet.Read() for packet {packet.GetType().FullName}! Maybe the incoming packet and the expected packet differ?",
						e);
					// WHEN ISSUING THIS EXCEPTION ENSURE THAT THE CURRENT STATE FOR THE PLAYER IS CORRECT!
				}

				if (input.ReadableBytes > 0)
				{
					NetworkBootstrap.Logger.Warn(
						$"Incoming packet {packet.GetType().FullName} has more bytes than expected!");
				}
			

				output.Add(packet);
			}
			finally
			{
				input.Clear(); // CLEAR INDEX SO THAT THE NEXT PACKETS DON'T GET MESSED UP. # Thanks to @turulix
			}
		}
	}
}