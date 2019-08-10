using System;
using DotNetty.Buffers;
using Recube.Api.Network.Entities;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Play
{
	// https://wiki.vg/Protocol#Spawn_Object
	[Packet(0x00, NetworkPlayerState.Play)]
	public class SpawnObjectPacket : IOutPacket
	{
		public int EntityID;
		public UUID UUID;
		public byte Type;
		public double X;
		public double Y;
		public double Z;
		public int Pitch;
		public int Yaw;
		public int Data;
		public short VelocityX;
		public short VelocityY;
		public short VelocityZ;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(EntityID);
			buffer.WriteStringWithLength(UUID.ToString());
			buffer.WriteByte(Type);
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
			buffer.WriteByte((byte) MathF.Floor(Pitch * 256.0F / 360.0F));
			buffer.WriteByte((byte) MathF.Floor(Yaw * 256.0F / 360.0F));
			buffer.WriteInt(Data);
			buffer.WriteShort(VelocityX);
			buffer.WriteShort(VelocityY);
			buffer.WriteShort(VelocityZ);
		}
	}
}