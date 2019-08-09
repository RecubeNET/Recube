using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;
using Recube.Core.Network.Pipeline;

namespace Recube.Core.Network
{
	public class NetworkBootstrap
	{
		public static readonly ILogger Logger = LogManager.GetLogger("Network");

		private IEventLoopGroup _bossGroup;
		private IEventLoopGroup _workerGroup;

		public bool Active { get; private set; }

		public async Task StartAsync(IPEndPoint address)
		{
			if (Active) return;
			Active = true;
			Logger.Info($"Starting network on {address.Address}:{address.Port}");
			try
			{
				_bossGroup = new MultithreadEventLoopGroup(1);
				_workerGroup = new MultithreadEventLoopGroup();

				var bootstrap = new ServerBootstrap();
				bootstrap.Group(_bossGroup, _workerGroup);
				bootstrap.Channel<TcpServerSocketChannel>();


				bootstrap
					.Option(ChannelOption.SoBacklog, 128)
					.ChildOption(ChannelOption.TcpNodelay, true)
					.ChildOption(ChannelOption.SoKeepalive, true)
					.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
					{
						var pipeline = channel.Pipeline;
						pipeline.AddLast("read_timeout", new ReadTimeoutHandler(30));
						pipeline.AddLast("frame_decoder", new FrameDecoder());
						pipeline.AddLast("packet_decoder", new PacketDecoder());
						pipeline.AddLast("frame_encoder", new FrameEncoder());
						pipeline.AddLast("packet_encoder", new PacketEncoder());
						pipeline.AddLast("write_timeout", new WriteTimeoutHandler(30));
						pipeline.AddLast("inbound_handler", new PacketInboundHandler());
					}));

				var boundChannel = await bootstrap.BindAsync(address).ConfigureAwait(false);

				while (Active) await Task.Delay(1000).ConfigureAwait(false);
				await boundChannel.CloseAsync().ConfigureAwait(false);
				Logger.Info("Network closed");
			}
			catch (Exception exe)
			{
				throw new BootstrapConnectionClosedException("Bootstrap connection has been unexpectedly closed", exe);
			}
			finally
			{
				await _workerGroup.ShutdownGracefullyAsync();
				await _bossGroup.ShutdownGracefullyAsync();
			}
		}

		public void Stop()
		{
			if (!Active) return;
			Active = false;
		}
	}
}