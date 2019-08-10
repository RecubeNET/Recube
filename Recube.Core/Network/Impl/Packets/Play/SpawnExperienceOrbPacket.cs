using System;
using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Play
{
	[Packet(0x01, NetworkPlayerState.Play)]
	public class SpawnExperienceOrbPacket: IOutPacket
	{
		public int EntityID;
		public double X;
		public double Y;
		public double Z;
		public short Count;
		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(EntityID);
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
			buffer.WriteShort(Count);
		}
	}
}