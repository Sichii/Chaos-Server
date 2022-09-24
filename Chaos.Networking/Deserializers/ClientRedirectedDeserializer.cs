using System.Text;
using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record ClientRedirectedDeserializer : ClientPacketDeserializer<ClientRedirectedArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ClientRedirected;

    public override ClientRedirectedArgs Deserialize(ref SpanReader reader)
    {
        var seed = reader.ReadByte();
        var key = reader.ReadString8();
        var name = reader.ReadString8();
        var id = reader.ReadUInt32();

        return new ClientRedirectedArgs(
            seed,
            Encoding.ASCII.GetBytes(key),
            name,
            id);
    }
}