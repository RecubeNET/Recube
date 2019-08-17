using System;
using DotNetty.Buffers;
using Recube.Api.Network.Entities;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	//https://wiki.vg/Protocol#Spawn_Mob
	/// <summary>
	///     Used to spawn Mobs
	/// </summary>
	[Packet(0x03, NetworkPlayerState.Play)]
	public class SpawnMobPacket : IOutPacket
	{
		/// <summary>
		///     All Entity's you can spawn with this packet.
		/// </summary>
		public enum SpawnType
		{
			Bat = 3,
			Blaze = 4,
			CaveSpider = 6,
			Chicken = 7,
			Cod = 8,
			Cow = 9,
			Creeper = 10,
			Donkey = 11,
			Dolphin = 12,
			Drowned = 14,
			ElderGuardian = 15,
			EnderDragon = 17,
			Enderman = 18,
			Endermite = 19,
			Evoker = 21,
			Ghast = 26,
			Giant = 27,
			Guardian = 28,
			Horse = 29,
			Husk = 30,
			Illusioner = 31,
			Llama = 36,
			MagmaCube = 38,
			Mute = 46,
			MushroomCow = 47,
			Ocelot = 48,
			Parrot = 50,
			Pig = 51,
			Pufferfish = 52,
			PigZombie = 53,
			PolarBear = 54,
			Rabbit = 56,
			Salmon = 57,
			Sheep = 58,
			Shulker = 59,
			Silverfish = 61,
			Skeleton = 62,
			SkeletonHorse = 63,
			Slime = 67,
			SnowMan = 66,
			Spider = 69,
			Squid = 70,
			Stray = 71,
			TropicalFish = 72,
			Turtle = 73,
			Vex = 78,
			Villager = 79,
			IronGolem = 80,
			Vindicator = 81,
			Witch = 82,
			WitherBoss = 83,
			WitherSkeleton = 84,
			Wolf = 86,
			Zombie = 87,
			ZombieHorse = 88,
			ZombieVillager = 89,
			Phantom = 90
		}

		/// <summary>
		///     Entity ID
		/// </summary>
		public int EntityID;

		/// <summary>
		///     Head Pitch
		/// </summary>
		public int HeadPitch;

		//TODO: Metadata Its not even a string!
		/// <summary>
		///     MetaData of the Entity Not yet implemented
		/// </summary>
		public string Metadata;

		/// <summary>
		///     Entity Pitch
		/// </summary>
		public int Pitch;

		/// <summary>
		///     Entity Type <see cref="SpawnMobPacket.SpawnType" />
		/// </summary>
		public SpawnType Type;

		/// <summary>
		///     Entity UUID
		/// </summary>
		public UUID UUID;

		/// <summary>
		///     Velocity X
		/// </summary>
		public short VelocityX;

		/// <summary>
		///     Velocity Y
		/// </summary>
		public short VelocityY;

		/// <summary>
		///     Velocity Z
		/// </summary>
		public short VelocityZ;

		/// <summary>
		///     PositionX
		/// </summary>
		public double X;

		/// <summary>
		///     PositionY
		/// </summary>
		public double Y;

		/// <summary>
		///     Entity Yaw
		/// </summary>
		public int Yaw;

		/// <summary>
		///     PositionZ
		/// </summary>
		public double Z;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(EntityID);
			buffer.WriteStringWithLength(UUID.ToString());
			buffer.WriteVarInt((int) Type);
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
			buffer.WriteByte((byte) MathF.Floor(Yaw * 256.0F / 360.0F));
			buffer.WriteByte((byte) MathF.Floor(Pitch * 256.0F / 360.0F));
			buffer.WriteByte((byte) MathF.Floor(HeadPitch * 256.0F / 360.0F));
			buffer.WriteShort(VelocityX);
			buffer.WriteShort(VelocityY);
			buffer.WriteShort(VelocityZ);
			//TODO Metadata!
			buffer.WriteStringWithLength(Metadata);
		}
	}
}