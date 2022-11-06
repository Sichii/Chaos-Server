using System.Text;

namespace Chaos.Packets.Abstractions;

public interface IPacketSerializer
{
    Encoding Encoding { get; }

    T Deserialize<T>(in ClientPacket packet) where T: IReceiveArgs;
    ServerPacket Serialize<T>(T obj) where T: ISendArgs;
}