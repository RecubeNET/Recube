using System;
using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Login
{
	[Packet(0x01, NetworkPlayerState.Login)]
	public class EncryptionResponsePacket : IInPacket
	{
		public int SharedSecretLength;
		public byte[] SharedSecret;
		public int VerifyTokenLength;
		public byte[] VerifyToken;
		public void Read(IByteBuffer buffer)
		{
			SharedSecretLength = buffer.ReadVarInt();
			SharedSecret = buffer.ReadBytes(SharedSecretLength).Array;
			VerifyTokenLength = buffer.ReadVarInt();
			VerifyToken = buffer.ReadBytes(VerifyTokenLength).Array;
		}
	}
}