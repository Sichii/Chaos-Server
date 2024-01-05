using System.Text;
using Chaos.IO.Memory;
using Chaos.Packets;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Utilities;

public static class ServerPacketEx
{
    public static Packet FromData(ServerOpCode opCode, Encoding encoding, params byte[] data)
    {
        var packet = new Packet(opCode);

        if (data.Length > 0)
        {
            var writer = new SpanWriter(encoding, data.Length);
            writer.WriteBytes(data);
            packet.Buffer = writer.ToSpan();
        }

        return packet;
    }
}