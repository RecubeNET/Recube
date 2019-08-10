using System;
using DotNetty.Buffers;
using Recube.Api.Network.Entities;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Core.Network.Impl.Packets.Play
{
	// https://wiki.vg/Protocol#Spawn_Object
	/// <summary>
	///     Used to Spawn Objects
	///     <para> Not used to spawn XP, Mobs, Players, Paintings, Thunderbolts </para>
	/// </summary>
	[Packet(0x00, NetworkPlayerState.Play)]
	public class SpawnObjectPacket : IOutPacket
	{
		/// <summary>
		///     All Entity's spawned with this packet.
		/// </summary>
		public enum SpawnType
		{
			Boat = 1,
			ItemStack = 2,
			AreaEffectCloud = 3,
			Minecart = 10,
			ActivatedTnt = 50,
			EnderCrystal = 51,
			TippedArrow = 60,
			Snowball = 61,
			Egg = 62,
			FireBall = 63,
			FireCharge = 64,
			ThrownEnderpearl = 65,
			WitherSkull = 66,
			ShulkerBullet = 67,
			LlamaSpit = 68,
			FallingObjects = 70,
			ItemFrame = 71,
			EyeOfEnder = 72,
			ThrownPotion = 73,
			ThrownExpBottle = 75,
			FireworkRocket = 76,
			LeashKnot = 77,
			ArmorStand = 78,
			EvocationFangs = 79,
			FishingHook = 90,
			SpectralArrow = 91,
			DragonFireball = 93,
			Trident = 94
		}

		/// <summary>
		///     Meaning dependent on the value of the Type field
		/// </summary>
		public int Data;

		/// <summary>
		///     ID of the Entity
		/// </summary>
		public int EntityID;

		/// <summary>
		///     Entity Pitch
		/// </summary>
		public int Pitch;

		/// <summary>
		///     The Type of the Objcets <see cref="SpawnObjectPacket.SpawnType" />
		/// </summary>
		public SpawnType Type;

		/// <summary>
		///     UUID of the Entity
		/// </summary>
		public UUID UUID;

		/// <summary>
		///     Entity VelocityX
		/// </summary>
		public short VelocityX;

		/// <summary>
		///     Entity VelocityY
		/// </summary>
		public short VelocityY;

		/// <summary>
		///     Entity VelocityZ
		/// </summary>
		public short VelocityZ;

		/// <summary>
		///     Position X
		/// </summary>
		public double X;

		/// <summary>
		///     Position Y
		/// </summary>
		public double Y;

		/// <summary>
		///     Entity Yaw
		/// </summary>
		public int Yaw;

		/// <summary>
		///     Position Z
		/// </summary>
		public double Z;

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