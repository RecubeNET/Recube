using System;
using System.Net;
using System.Threading.Tasks;
using Recube.Api;
using Recube.Api.Network.Impl.Packets.Play;
using Recube.Core.Block;

namespace Recube.Core
{
    public partial class Recube
    {
        private static void Main() => new Recube(); // TODO ADD ARGS FOR PORT ETC.

        public Recube()
        {
            Instance = this;
            RecubeApi.SetRecubeInstance(this);

            Logger.Info("Starting Recube...");

            ListenerRegistry.RegisterAll(typeof(TestListeners));

            RegisterPackets();

            var blockDic = BlockParser.ParseFile("blocks_1.15.1.json").GetAwaiter().GetResult();
            var parsedBlocks = BlockParser.ParseBlockClasses("Recube.Api.Block.Impl");
            BlockStateRegistry = new BlockStateRegistry(blockDic, parsedBlocks);
            Logger.Info($"Parsed {blockDic.Count} blocks");

            WorldThread.AddWorld(TestWorld);
            WorldThread.Start();


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
    }
}