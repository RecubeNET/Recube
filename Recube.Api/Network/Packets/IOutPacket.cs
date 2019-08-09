using DotNetty.Buffers;

namespace Recube.Api.Network.Packets
{
	/// <summary>
	/// The parent class for every outgoing packet
	/// </summary>
	public interface IOutPacket : IPacket
	{
		/// <summary>
		/// Gets called to encode a packet
		/// </summary>
		/// <param name="buffer">The to-write buffer</param>
		void Write(IByteBuffer buffer);
	}
}