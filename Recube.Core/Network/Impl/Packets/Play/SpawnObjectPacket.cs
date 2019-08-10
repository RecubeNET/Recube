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
		public enum SpawnType
		{
			Boat=1,
			ItemStack=2,
			AreaEffectCloud=3,
			Minecart=10,
			ActivatedTNT=50,
			EnderCrystal=51,
			TippedArrow=60,
			Snowball=61,
			Egg=62,
			FireBall=63,
			FireCharge=64,
			ThrownEnderpearl=65,
			WitherSkull=66,
			ShulkerBullet=67,
			LlamaSpit=68,
			FallingObjects=70,
			ItemFrame=71,
			EyeOfEnder=72,
			ThrownPotion=73,
			ThrownExpBottle=75,
			FireworkRocket=76,
			LeashKnot=77,
			ArmorStand=78,
			EvocationFangs=79,
			FishingHook=90,
			SpectralArrow=91,
			DragonFireball=93,
			Trident=94
		}
		
		public int EntityID;
		public UUID UUID;
		public SpawnType Type;
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
			buffer.WriteByte((byte) Type);
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