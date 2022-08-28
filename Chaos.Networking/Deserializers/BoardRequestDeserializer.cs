using Chaos.Common.Definitions;
using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
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