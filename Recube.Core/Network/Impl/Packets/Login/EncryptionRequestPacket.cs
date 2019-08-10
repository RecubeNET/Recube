using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Login
{
	[Packet(0x01, NetworkPlayerState.Login)]
	public class EncryptionRequestPacket : IOutPacket
	{
		public byte[] PublicKey;
		public int PublicKeyLength;
		public string ServerID = "";
		public byte[] VerifyToken = {1, 0, 0, 1};
		public int VerifyTokenLength = 4;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteStringWithLength(ServerID);
			buffer.WriteVarInt(PublicKeyLength);
			buffer.WriteBytes(PublicKey);
			buffer.WriteVarInt(VerifyTokenLength);
			buffer.WriteBytes(VerifyToken);
		}
	}
}