using System;
using DotNetty.Buffers;
using Recube.Api.Network.Entities;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.Packets;
using Recube.Api.Network.NetworkPlayer;

namespace Recube.Core.Network.Impl.Packets.Play
{
	/// <summary>
	/// This packet is used to spawn a player when hes in view range.
	/// <para>DO NOT USE THIS TO SPAWN A PLAYER ON CONNECT</para>
	/// </summary>
	[Packet(0x05, NetworkPlayerState.Play)]
	public class SpawnPlayerPacket : IOutPacket
	{
		/// <summary>
		/// Player ID
		/// </summary>
		public int EntityID;

		/// <summary>
		/// Player UUID
		/// </summary>
		public UUID UUID;

		/// <summary>
		/// PositionX
		/// </summary>
		public double X;

		/// <summary>
		/// PositionY
		/// </summary>
		public double Y;

		/// <summary>
		/// PositionZ
		/// </summary>
		public double Z;

		/// <summary>
		/// Player Yaw
		/// </summary>
		public int Yaw;

		/// <summary>
		/// Player Pitch
		/// </summary>
		public int Pitch;

		//TODO: Metadata Its not even a string!
		/// <summary>
		/// MetaData of the Player Not yet implemented
		/// </summary>
		public string Metadata;
		
		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(EntityID);
			buffer.WriteStringWithLength(UUID.ToString());
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
			buffer.WriteInt(Yaw);
			buffer.WriteInt(Pitch);
			//TODO: Metadata implementation
			buffer.WriteStringWithLength(Metadata);
		}
	}
}