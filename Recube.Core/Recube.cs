using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Recube.Api;
using Recube.Api.Entities;
using Recube.Api.World;
using Recube.Core.Block;
using Recube.Core.Entities;
using Recube.Core.Network;
using Recube.Core.Network.Impl;
using Recube.Core.Network.NetworkPlayer;
using Recube.Core.World;

namespace Recube.Core
{
	public partial class Recube : IRecube
	{
		public const int ProtocolVersion = 498;
		public static readonly ILogger RecubeLogger = LogManager.GetLogger("Recube");
		public readonly BlockStateRegistry BlockStateRegistry = new BlockStateRegistry();

		public readonly EntityRegistry EntityRegistry = new EntityRegistry();

		public readonly Type HandshakePacketHandler = typeof(HandshakePacketHandler);
		public readonly Type LoginPacketHandler = typeof(LoginPacketHandler);

		public readonly NetworkBootstrap NetworkBootstrap = new NetworkBootstrap();
		public readonly NetworkPlayerRegistry NetworkPlayerRegistry = new NetworkPlayerRegistry();
		public readonly PlayerRegistry PlayerRegistry = new PlayerRegistry();
		public readonly Type PlayPacketHandler = typeof(PlayPacketHandler);
		public readonly Type StatusPacketHandler = typeof(StatusPacketHandler);


		public string Motd = @"{
	""version"": {
		""name"": ""1.13.2"",
		""protocol"": 404
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
			IWorld world = new RecubeWorld("World");
			world.SaveWorld();
			world.LoadWorld();

			var a = new BlockParser("blocks_1.14.4.json").Parse().GetAwaiter().GetResult();
			foreach (var keyValuePair in a)
			{
				BlockStateRegistry.Register(keyValuePair.Key.Name, keyValuePair.Value);
			}

			foreach (var keyValuePair in BlockStateRegistry.GetAll())
			{
				var c = new StringBuilder($"{keyValuePair.Key}: \n");
				foreach (var blockState in keyValuePair.Value)
				{
					var dw = new StringBuilder();
					if (blockState.Properties != null)
					{
						foreach (var blockStateProperty in blockState.Properties)
						{
							dw.AppendJoin(", ", $"{blockStateProperty.Key}: {blockStateProperty.Value}");
						}
					}
					else
					{
						dw.Append("NO PROPS");
					}

					c.Append($"\tID: {blockState.Id}; DEFAULT: {blockState.Default}; PROPS: {dw}\n");
				}

				Console.WriteLine(c.ToString());
			}


			Task.Run(() => NetworkBootstrap.StartAsync(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 25565)));

			while (true) Console.ReadLine(); // TEMPORARY SOLUTION
		}

		public static Recube Instance { get; private set; }

		public ILogger Logger => RecubeLogger;

		public IPlayerRegistry GetPlayerRegistry() => PlayerRegistry;

		public IEntityRegistry GetEntityRegistry() => EntityRegistry;

		private static void Main() => new Recube(); // TODO ADD ARGS FOR PORT ETC.
	}
}