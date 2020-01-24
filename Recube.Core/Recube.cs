using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Recube.Api;
using Recube.Api.Block;
using Recube.Api.Entities;
using Recube.Api.Network.Impl.Packets.Play;
using Recube.Core.Block;
using Recube.Core.Entities;
using Recube.Core.Network;
using Recube.Core.Network.Impl;
using Recube.Core.Network.NetworkPlayer;

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
		""name"": ""1.15.2"",
		""protocol"": 575
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

			var a = new BlockParser("blocks_1.15.1.json").Parse().GetAwaiter().GetResult();
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

			Console.CancelKeyPress += async (sender, e) =>
			{
				e.Cancel = true;
				var disconnectPacket = new DisconnectOutPacket {Reason = "Recube closed"};
				foreach (var player in PlayerRegistry.GetAll())
				{
					await player.NetworkPlayer.SendPacketAsync(disconnectPacket);
					await player.NetworkPlayer.DisconnectAsync();
				}
				NetworkBootstrap.Stop();
				Environment.Exit(0);
			};
			
			while (true) Console.ReadLine(); // TEMPORARY SOLUTION
		}

		public static Recube Instance { get; private set; }

		public ILogger Logger => RecubeLogger;

		public IPlayerRegistry GetPlayerRegistry() => PlayerRegistry;

		public IEntityRegistry GetEntityRegistry() => EntityRegistry;
		public IBlockStateRegistry GetBlockStateRegistry() => BlockStateRegistry;

		private static void Main() => new Recube(); // TODO ADD ARGS FOR PORT ETC.
	}
}