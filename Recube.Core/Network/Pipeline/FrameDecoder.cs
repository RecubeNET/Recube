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
			input.MarkReaderIndex();

			VarInt.ReadVarInt(input, out var length);

			if (length == null || input.ReadableBytes < length)
			{
				input.ResetReaderIndex();
				return;
			}

			var buf = input.Slice(input.ReaderIndex, length.Value);
			buf.Retain();
			input.SetReaderIndex(input.ReaderIndex + length.Value);
			output.Add(buf);
		}
	}
}