using Chaos.Core.Synchronization;
using Chaos.Cryptography.Abstractions;
using Chaos.Entities.Networking;
using Chaos.Packets;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Abstractions;

public interface ISocketClient
{
    ICryptoClient CryptoClient { get; set; }
    event EventHandler? OnDisconnected;
    bool Connected { get; }
    uint Id { get; }
    FifoSemaphoreSlim ReceiveSync { get; }

    void BeginReceive();
    void Disconnect();
    bool IsLoopback();
    void Send<T>(T obj) where T: ISendArgs;
    void Send(ref ServerPacket packet);
    void SendAcceptConnection();
    void SendHeartBeat(byte first, byte second);
    void SendRedirect(Redirect redirect);
    void SetSequence(byte newSequence);
}