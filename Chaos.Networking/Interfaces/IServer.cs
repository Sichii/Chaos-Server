using Chaos.Packets;

namespace Chaos.Networking.Interfaces;

public interface IServer
{
    ValueTask HandlePacketAsync<TClient>(TClient client, ref ClientPacket packet) where TClient: ISocketClient;
    ValueTask OnHeartBeatAsync(ISocketClient client, ref ClientPacket packet);
    ValueTask OnSequenceChangeAsync(ISocketClient client, ref ClientPacket packet);
    ValueTask OnSynchronizeTicksAsync(ISocketClient client, ref ClientPacket packet);

    void Start();
}