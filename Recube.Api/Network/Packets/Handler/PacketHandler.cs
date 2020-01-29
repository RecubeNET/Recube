using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Recube.Api.Network.NetworkPlayer;

namespace Recube.Api.Network.Packets.Handler
{
    /// <summary>
    ///     The heart of every network connection.
    ///     It's main job is to handle all incoming packets.
    ///     Can be switched out via <see cref="INetworkPlayer.SetPacketHandler" /> for handling efficiently
    ///     multiple states (for example login state & play state)
    /// </summary>
    /// <example>
    ///     To listen for specific packets just add these methods to your class.
    ///     <b>Warning: Methods have to be public</b>
    ///     <code>
    ///  [PacketMethod]
    ///  public void OnYourPacket(YourPacket packet)
    ///  {
    ///  	// HANDLE PACKET
    ///  	// FOR EXAMPLE:
    ///  	NetworkPlayer.SendPacketAsync(new YourOutPacket("hello world"));
    ///  }
    /// 
    ///  [PacketMethod]
    ///  public void OnYourSecondPacketType(YourSecondPacket packet)
    ///  {
    ///  	// HANDLE PACKET
    ///  }
    ///  </code>
    /// </example>
    public abstract class PacketHandler
    {
        protected INetworkPlayer NetworkPlayer;

        /// <summary>
        ///     <b>Warning: Do not add more parameters due to the fact that this class is constructed via reflections</b>
        /// </summary>
        /// <param name="networkPlayer">The player</param>
        protected PacketHandler(INetworkPlayer networkPlayer)
        {
            NetworkPlayer = networkPlayer;
        }

        /// <summary>
        ///     Called when the PacketHandler gets set.
        ///     If it's the first PacketHandler, then it also means that a client connected.
        /// </summary>
        public abstract void OnActive();

        /// <summary>
        ///     Called when a client disconnects.
        /// </summary>
        public abstract void OnDisconnect();

        /// <summary>
        ///     Called when an incoming packet has no specified method.
        /// </summary>
        /// <param name="packet">The packet</param>
        [PacketMethod]
        public abstract Task Fallback(IInPacket packet);

        /// <summary>
        ///     Gets called on an incoming packet.
        ///     Usually called by the default InboundHandler
        /// </summary>
        /// <param name="packet">The incoming packet</param>
        /// <returns>The task or a new one if the return type is not Task</returns>
        /// <exception cref="NullReferenceException">Called when the Fallback method is null</exception>
        public Task FirePacket(IInPacket packet)
        {
            var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(typeof(PacketMethodAttribute), false).Length > 0)
                .ToImmutableArray();
            var packetMethod =
                methods.FirstOrDefault(m => m.GetParameters().All(p => p.ParameterType == packet.GetType()));
            if (packetMethod == null)
                packetMethod = GetType().GetMethod(nameof(Fallback));

            if (packetMethod == null) throw new NullReferenceException("Fallback method is null");

            var ret = packetMethod.Invoke(this, new object[] {packet});

            if (ret != null && ret is Task task)
                return task;

            return Task.CompletedTask;
        }
    }
}