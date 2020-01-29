using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
    [Packet(0x08, NetworkPlayerState.Play)]
    public class AcknowledgePlayerDiggingOutPacket : IOutPacket
    {
        public long Location; // TODO
        public int BlockType; // TODO
        public StatusEnum Status;
        public bool Successful;

        public void Write(IByteBuffer buffer)
        {
            buffer.WriteLong(Location);
            buffer.WriteVarInt(BlockType);
            buffer.WriteVarInt((int) Status);
            buffer.WriteBoolean(Successful);
        }

        public enum StatusEnum
        {
            StartedDigging = 0,
            CancelledDigging = 1,
            FinishedDigging = 2,
        }
    }
}