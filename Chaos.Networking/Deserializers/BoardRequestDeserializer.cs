using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record BoardRequestDeserializer : ClientPacketDeserializer<BoardRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.BoardRequest;

    public override BoardRequestArgs Deserialize(ref SpanReader reader)
    {
        var boardRequestType = (BoardRequestType)reader.ReadByte();

        return new BoardRequestArgs(boardRequestType);
    }
}