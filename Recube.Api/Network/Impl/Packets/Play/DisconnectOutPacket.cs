using System;
using System.Text;
using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
    [Packet(0x1B, NetworkPlayerState.Play)]
    public class DisconnectOutPacket : IOutPacket
    {
        // TODO USE REAL CHAT
        public string Reason;

        public void Write(IByteBuffer buffer)
        {
            var s = @$"{{""text"": ""{Reason}""}}";
            buffer.WriteVarInt(s.Length);
            buffer.WriteString(s, Encoding.UTF8);
        }
    }
}