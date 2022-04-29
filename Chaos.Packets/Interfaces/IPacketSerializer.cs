using System.Text;

namespace Chaos.Packets.Interfaces;

public interface IPacketSerializer
{
    Encoding Encoding { get; }

    T Deserialize<T>(ref ClientPacket packet) where T: IReceiveArgs;
    ServerPacket Serialize<T>(T obj) where T: ISendArgs;
}