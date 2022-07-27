using System.Text;
using Chaos.IO.Memory;
using Chaos.Packets.Definitions;

namespace Chaos.Utilities;

public static class ServerPacket
{
    public static Packets.ServerPacket FromData(ServerOpCode opCode, Encoding? encoding = null, params byte[] data)
    {
        var packet = new Packets.ServerPacket(opCode);

        if (data.Length > 0)
        {
            var writer = new SpanWriter(encoding!, data.Length);
            writer.WriteBytes(data);
            packet.Buffer = writer.ToSpan();
        }

        return packet;
    }
}