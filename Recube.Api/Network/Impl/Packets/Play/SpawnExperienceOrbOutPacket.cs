using DotNetty.Buffers;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;

namespace Recube.Api.Network.Impl.Packets.Play
{
	/// <summary>
	///     Used to spawn ExperienceOrbs
	/// </summary>
	[Packet(0x01, NetworkPlayerState.Play)]
	public class SpawnExperienceOrbOutPacket : IOutPacket
	{
		/// <summary>
		///     Ammount of XP orbs
		/// </summary>
		public short Count;

		/// <summary>
		///     Entity ID
		/// </summary>
		public int EntityId;

		/// <summary>
		///     Position X
		/// </summary>
		public double X;

		/// <summary>
		///     PositionY
		/// </summary>
		public double Y;

		/// <summary>
		///     Position Z
		/// </summary>
		public double Z;

		public void Write(IByteBuffer buffer)
		{
			buffer.WriteVarInt(EntityId);
			buffer.WriteDouble(X);
			buffer.WriteDouble(Y);
			buffer.WriteDouble(Z);
			buffer.WriteShort(Count);
		}
	}
}