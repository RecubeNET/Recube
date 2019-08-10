using System;
using System.Net;
using System.Threading.Tasks;
using NLog;
using Recube.Api;
using Recube.Core.Network;
using Recube.Core.Network.Impl;
using Recube.Core.Network.NetworkPlayer;

namespace Recube.Core
{
	public partial class Recube : IRecube
	{
		public readonly static int ProtocolVersion = 498;
		public readonly static ILogger RecubeLogger = LogManager.GetLogger("Recube");

		public readonly Type HandshakePacketHandler = typeof(HandshakePacketHandler);
		public readonly Type LoginPacketHandler = typeof(LoginPacketHandler);


		public readonly NetworkBootstrap NetworkBootstrap = new NetworkBootstrap();
		public readonly NetworkPlayerRegistry NetworkPlayerRegistry = new NetworkPlayerRegistry();
		public readonly Type PlayPacketHandler = typeof(PlayPacketHandler);
		public readonly Type StatusPacketHandler = typeof(StatusPacketHandler);

		public string Motd = @"{
	""version"": {
		""name"": ""1.14.4"",
		""protocol"": 498
	},
	""players"": {
		""max"": 100,
		""online"": 0,
		""sample"": [
		]
	},	
	""description"": {
		""text"": ""Running on Recube""
	},
	""favicon"": ""data:image/png;base64,<data>""
}";

		public Recube()
		{
			Instance = this;

			Logger.Info("Starting Recube...");

			RegisterPackets();

			Task.Run(() => NetworkBootstrap.StartAsync(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 25565)));

			while (true) Console.ReadLine(); // TEMPORARY SOLUTION
		}

		public static Recube Instance { get; private set; }

		public ILogger Logger => RecubeLogger;

		static void Main() => new Recube(); // TODO ADD ARGS FOR PORT ETC.
	}
}