using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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