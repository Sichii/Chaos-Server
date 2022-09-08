using Chaos.Packets;
using Microsoft.Extensions.Hosting;

namespace Chaos.Networking.Abstractions;

public interface IServer : IHostedService
{
    ValueTask HandlePacketAsync(ISocketClient client, ref ClientPacket packet);
    ValueTask OnHeartBeatAsync(ISocketClient client, ref ClientPacket packet);
    ValueTask OnSequenceChangeAsync(ISocketClient client, ref ClientPacket packet);
    ValueTask OnSynchronizeTicksAsync(ISocketClient client, ref ClientPacket packet);
}