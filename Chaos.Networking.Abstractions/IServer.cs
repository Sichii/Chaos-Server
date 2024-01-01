using Chaos.Packets;
using Microsoft.Extensions.Hosting;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines the bare minimum for a server
/// </summary>
public interface IServer<in TClient> : IHostedService where TClient: ISocketClient
{
    /// <summary>
    ///     A catch-all that will re-route a packet to the correct handler
    /// </summary>
    ValueTask HandlePacketAsync(TClient client, in ClientPacket packet);

    /// <summary>
    ///     A client is notifying the server that it encountered an exception from a bad packet
    /// </summary>
    ValueTask OnClientException(TClient client, in ClientPacket packet);

    /// <summary>
    ///     A client has sent a heartbeat(keep-alive) ping
    /// </summary>
    ValueTask OnHeartBeatAsync(TClient client, in ClientPacket packet);

    /// <summary>
    ///     A client has requested to change the packet sequence number
    /// </summary>
    ValueTask OnSequenceChangeAsync(TClient client, in ClientPacket packet);

    /// <summary>
    ///     A client has sent it's Environment.Ticks value
    /// </summary>
    ValueTask OnSynchronizeTicksAsync(TClient client, in ClientPacket packet);
}