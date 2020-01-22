using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Util;

namespace Recube.Api.Network.Impl.Packets.Play
{
	/// <summary>
	///     This packet is used to spawn a player when hes in view range.
	///     <para>DO NOT USE THIS TO SPAWN A PLAYER ON CONNECT</para>
	/// </summary>
	[Packet(0x05, NetworkPlayerState.Play)]
	public class SpawnPlayerOutPacket : IOutPacket
	{
		/// <summary>
		///     Player ID
		/// </summary>
		public int EntityId;

		/// <summary>
		///     Player Pitch
		/// </summary>
		public int Pitch;

		/// <summary>
		///     Player UUID
		/// </summary>
		public Uuid Uuid;

		/// <summary>
		///     PositionX
		/// </summary>
		public double X;

		/// <summary>
		///     PositionY
		/// </summary>
		public double Y;

		/// <summary>
		///     Player Yaw
		/// </summary>
		public int Yaw;

		/// <summary>
		///     PositionZ
		/// </summary>
		public double Z;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(EntityId);
			buffer.WriteStringWithLength(Uuid.ToString());
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
			buffer.WriteInt(Yaw);
			buffer.WriteInt(Pitch);
		}
	}
}