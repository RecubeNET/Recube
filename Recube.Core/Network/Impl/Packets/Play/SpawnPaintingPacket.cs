using System;
using DotNetty.Buffers;
using Recube.Api.Network.Entities;
using Recube.Api.Network.Packets;
using Recube.Api.Network.NetworkPlayer;

namespace Recube.Core.Network.Impl.Packets.Play
{
	/// <summary>
	/// Used to spawn paintings
	/// </summary>
	[Packet(0x04, NetworkPlayerState.Play)]
	public class SpawnPaintingPacket : IOutPacket
	{
		/// <summary>
		/// Directions the painting can face.
		/// </summary>
		public enum Directions
		{
			North=2,
			South=0,
			West=1,
			East=3
		}
		
		/// <summary>
		/// EntityID
		/// </summary>
		public int EntityID;
		/// <summary>
		/// Entity UUID
		/// </summary>
		public UUID UUID;
		/// <summary>
		/// Motive of the Picture
		/// </summary>
		public int Motive;
		//TODO: Position (x 26bit int, Y 12bit int, Z 26bit int)
		/// <summary>
		/// Center Position of the picture
		/// </summary>
		public int Position;
		/// <summary>
		/// Direction the Entity is facing <see cref="Directions"/>
		/// </summary>
		public Directions Direction;
		public void Write(IByteBuffer buffer)
		{
			//TODO: Implement
			throw new NotImplementedException();
		}
	}
}