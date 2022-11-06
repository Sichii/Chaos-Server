using Chaos.Packets;
using Microsoft.Extensions.Hosting;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines the bare minimum for a server
/// </summary>
public interface IServer<in T> : IHostedService where T: ISocketClient
{
    ValueTask HandlePacketAsync(T client, in ClientPacket packet);
    ValueTask OnHeartBeatAsync(T client, in ClientPacket packet);
    ValueTask OnSequenceChangeAsync(T client, in ClientPacket packet);
    ValueTask OnSynchronizeTicksAsync(T client, in ClientPacket packet);
}