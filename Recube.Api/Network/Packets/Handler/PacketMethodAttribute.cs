using System;
using JetBrains.Annotations;

namespace Recube.Api.Network.Packets.Handler
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class PacketMethodAttribute : Attribute
    {
    }
}