using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Recube.Api.Network.Extensions;

namespace Recube.Core.Network.Pipeline
{
	public class FrameEncoder : MessageToByteEncoder<IByteBuffer>
	{
		protected override void Encode(IChannelHandlerContext context, IByteBuffer message, IByteBuffer output)
		{
			output.WriteVarInt(message.ReadableBytes);
			output.WriteBytes(message);
		}
	}
}