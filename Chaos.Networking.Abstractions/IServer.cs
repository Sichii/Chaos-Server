using Chaos.Packets;
using Microsoft.Extensions.Hosting;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents an instance of a server
/// </summary>
public interface IServer<in T> : IHostedService where T : ISocketClient
{
    ValueTask HandlePacketAsync(T client, ref ClientPacket packet);
    ValueTask OnHeartBeatAsync(T client, ref ClientPacket packet);
    ValueTask OnSequenceChangeAsync(T client, ref ClientPacket packet);
    ValueTask OnSynchronizeTicksAsync(T client, ref ClientPacket packet);
}