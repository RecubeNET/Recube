using System;
using System.Net;
using System.Threading.Tasks;
using Recube.Api.Block.Impl;
using Recube.Api.Network.Impl.Packets.Play;
using Recube.Core.Block;

namespace Recube.Core
{
    public partial class Recube
    {
        public Recube()
        {
            Instance = this;

            Logger.Info("Starting Recube...");

            ListenerRegistry.RegisterAll(typeof(TestListeners));

            RegisterPackets();

            var blockParser = new BlockParser("blocks_1.15.1.json");
            var blockDic = blockParser.ParseFile().GetAwaiter().GetResult();
            var parsedBlocks = blockParser.ParseBlockClasses();
            BlockStateRegistry = new BlockStateRegistry(blockDic, parsedBlocks);

            Console.WriteLine(BlockStateRegistry.GetStateByBaseBlock(new GrassBlock(GrassBlock.SnowyProperty.Default))
                .NetworkId);
            Console.WriteLine(BlockStateRegistry.GetStateByBaseBlock(new GrassBlock(GrassBlock.SnowyProperty.Snowy))
                .NetworkId);
            Console.WriteLine(BlockStateRegistry.GetStateByBaseBlock(new AirBlock()).NetworkId);
            Console.WriteLine(BlockStateRegistry.GetStateByBaseBlock(new CoalOreBlock()).NetworkId);

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