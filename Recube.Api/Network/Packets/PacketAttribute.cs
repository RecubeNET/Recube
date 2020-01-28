using System;
using JetBrains.Annotations;
using Recube.Api.Network.NetworkPlayer;

namespace Recube.Api.Network.Packets
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse]
    public class PacketAttribute : Attribute
    {
        public int Id;
        public NetworkPlayerState State;

        public PacketAttribute(int id, NetworkPlayerState state)
        {
            Id = id;
            State = state;
        }
    }
}