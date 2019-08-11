using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Login
{
	[Packet(0x01, NetworkPlayerState.Login)]
	public class EncryptionResponsePacket : IInPacket
	{
		public byte[] SharedSecret;
		public int SharedSecretLength;
		public byte[] VerifyToken;
		public int VerifyTokenLength;

		public void Read(IByteBuffer buffer)
		{
			SharedSecretLength = buffer.ReadVarInt();
			SharedSecret = buffer.ReadBytes(SharedSecretLength).Array;
			VerifyTokenLength = buffer.ReadVarInt();
			VerifyToken = buffer.ReadBytes(VerifyTokenLength).Array;
		}
	}
}