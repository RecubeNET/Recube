using System.Threading.Tasks;
using Recube.Api.Network.Impl.Packets.Status;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;

namespace Recube.Core.Network.Impl
{
    public class StatusPacketHandler : PacketHandler
    {
        public StatusPacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
        {
        }

        public override void OnActive()
        {
            ((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Status);
        }

        public override void OnDisconnect()
        {
        }

        public override Task Fallback(IInPacket packet)
        {
            return Task.CompletedTask;
        }

        [PacketMethod]
        public void OnRequestInPacket(RequestInPacket packet)
        {
            NetworkPlayer.SendPacketAsync(new ResponseOutPacket
            {
                JsonResponse = Recube.Instance.Motd
            });
        }

        [PacketMethod]
        public void OnPingInPacket(PingInPacket packet)
        {
            NetworkPlayer.SendPacketAsync(new PongOutPacket
            {
                Payload = packet.Payload
            });
        }
    }
}