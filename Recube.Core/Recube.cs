using System;
using System.Collections.Generic;
using NLog;
using Recube.Api;
using Recube.Api.Block;
using Recube.Api.Entities;
using Recube.Core.Block;
using Recube.Core.Entities;
using Recube.Core.Event;
using Recube.Core.Network;
using Recube.Core.Network.Impl;
using Recube.Core.Network.NetworkPlayer;
using Recube.Core.World;

namespace Recube.Core
{
    public partial class Recube : IRecube
    {
        public const int ProtocolVersion = 578;
        public static readonly ILogger RecubeLogger = LogManager.GetLogger("Recube");
        public readonly BlockStateRegistry BlockStateRegistry;

        public readonly EntityRegistry EntityRegistry = new EntityRegistry();

        public readonly Type HandshakePacketHandler = typeof(HandshakePacketHandler);
        public readonly Type LoginPacketHandler = typeof(LoginPacketHandler);

        public readonly NetworkBootstrap NetworkBootstrap = new NetworkBootstrap();
        public readonly NetworkPlayerRegistry NetworkPlayerRegistry = new NetworkPlayerRegistry();
        public readonly PlayerRegistry PlayerRegistry = new PlayerRegistry();
        public readonly Type PlayPacketHandler = typeof(PlayPacketHandler);
        public readonly Type StatusPacketHandler = typeof(StatusPacketHandler);

        public readonly ListenerRegistry ListenerRegistry = new ListenerRegistry();

        // TODO MAKE MULTIPLE WORLD THREADS POSSIBLE AND MULTIPLE WORLD FRIENDLY...
        public readonly WorldThread WorldThread = new WorldThread(0);
        public readonly World.World TestWorld = new World.World("tests", new List<Chunk> {new Chunk(0, 0)});

        public static Recube Instance { get; private set; }

        public ILogger Logger => RecubeLogger;

        public IPlayerRegistry GetPlayerRegistry() => PlayerRegistry;

        public IEntityRegistry GetEntityRegistry() => EntityRegistry;

        public IBlockStateRegistry GetBlockStateRegistry() => BlockStateRegistry;

        public string Motd = $@"{{
	""version"": {{
		""name"": ""1.15.2"",
		""protocol"": {ProtocolVersion}
	}},
	""players"": {{
		""max"": 100,
		""online"": 0,
		""sample"": [
		]
	}},	
	""description"": {{
		""text"": ""Running on Recube""
	}},
	""favicon"": ""data:image/png;base64,<data>""
}}";
    }
}