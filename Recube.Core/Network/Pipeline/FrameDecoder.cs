using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Recube.Api.Network;

namespace Recube.Core.Network.Pipeline
{
	public class FrameDecoder : ByteToMessageDecoder
	{
		protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
		{
			if (!VarInt.ReadVarInt(input.Array, out var length))
			{
				NetworkBootstrap.Logger.Warn("Received packet with invalid packet length!");
				return;
			}

			if (length == null) return;

			if (input.ReadableBytes < length)
			{
				NetworkBootstrap.Logger.Warn("Received packet with less bytes than expected!");
				return;
			}

			var buf = ByteBufferUtil.DefaultAllocator.Buffer(length.Value);
			input.ReadBytes(buf, length.Value);
			output.Add(buf);
		}
	}
}