using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
    [Packet(0x1A, NetworkPlayerState.Play)]
    public class PlayerDiggingInPacket : IInPacket
    {
        public int Status;
        public long Location; // TODO
        public byte Face;

        public void Read(IByteBuffer buffer)
        {
            Status = buffer.ReadVarInt();
            Location = buffer.ReadLong();
            Face = buffer.ReadByte();
        }
    }
}