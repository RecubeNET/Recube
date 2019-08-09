using System;
using System.Net;
using System.Threading.Tasks;
using NLog;
using Recube.Api;
using Recube.Core.Network;
using Recube.Core.Network.NetworkPlayer;
using Recube.Core.Network.Packets;

namespace Recube.Core
{
	public partial class Recube : IRecube
	{
		public readonly static ILogger RecubeLogger = LogManager.GetLogger("Recube");
		public readonly Type HandshakePacketHandler = typeof(HandshakePacketHandler);


		public readonly NetworkBootstrap NetworkBootstrap = new NetworkBootstrap();
		public readonly NetworkPlayerRegistry NetworkPlayerRegistry = new NetworkPlayerRegistry();

		public Recube()
		{
			Instance = this;

			Logger.Info("Starting Recube...");

			RegisterPackets();

			Task.Run(() => NetworkBootstrap.StartAsync(new IPEndPoint(IPAddress.Any, 25565)));

			while (true) Console.ReadLine(); // TEMPORARY SOLUTION
		}

		public static Recube Instance { get; private set; }

		public ILogger Logger => RecubeLogger;

		static void Main() => new Recube(); // TODO ADD ARGS FOR PORT ETC.
	}
}