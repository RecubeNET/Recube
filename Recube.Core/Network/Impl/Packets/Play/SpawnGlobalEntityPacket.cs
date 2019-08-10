using System;
using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.Packets;
using Recube.Api.Network.NetworkPlayer;

namespace Recube.Core.Network.Impl.Packets.Play
{
	/// <summary>
	/// Currently Only used to Spawn Thunderbolts.
	/// </summary>
	[Packet(0x02, NetworkPlayerState.Play)]
	public class SpawnGlobalEntity : IOutPacket
	{
		public enum SpawnType
		{
			Thunderbolt = 1
		}
		/// <summary>
		/// EntityID
		/// </summary>
		public int EntityID;
		/// <summary>
		/// The Type of the Objcets <see cref="SpawnGlobalEntity.SpawnType"/>
		/// </summary>
		public SpawnType Type;
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

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(EntityID);
			buffer.WriteByte((byte) Type);
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
		}
	}
}