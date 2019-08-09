using DotNetty.Buffers;

namespace Recube.Api.Network.Packets
{
	/// <summary>
	/// The parent class for every incoming packet
	/// </summary>
	public interface IInPacket : IPacket
	{
		/// <summary>
		/// Gets called to decode a packet
		/// </summary>
		/// <param name="buffer">The buffer containing all body bytes</param>
		void Read(IByteBuffer buffer);
	}
}