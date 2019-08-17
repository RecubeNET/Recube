namespace Recube.Api.Network.Packets
{
	/// <summary>
	///     This is used by the network part of Recube to determine the id of a packet.
	///     Note: The packet registry differs by the client-state and by the target of the packet(if it's outgoing or ingoing).
	///     Thanks Mojang for that.
	/// </summary>
	public interface IPacketRegistry
	{
		/// <summary>
		///     Adds a packet to the registry
		/// </summary>
		/// <param name="id">The packet id</param>
		/// <param name="packet">The packet which you want to register</param>
		/// <returns>False when a packet with the id already exists</returns>
		bool RegisterPacket<T>(int id, T packet) where T : IPacket;

		/// <summary>
		///     Removes a packet from the registry
		/// </summary>
		/// <param name="id">The packet's id to remove</param>
		/// <returns>True if the packet has been removed successfully</returns>
		bool DeregisterPacket(int id);

		/// <summary>
		///     Creates a clone of the packet class by it's id
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>Null when the packet with the id doesn't exist/has not yet been registered</returns>
		IPacket? GetPacketById(int id);

		/// <summary>
		///     Gets the id by a packet class
		/// </summary>
		/// <param name="packet">The packet</param>
		/// <returns>Null when the packet has not yet been registered</returns>
		int? GetPacketId(IPacket packet);
	}
}