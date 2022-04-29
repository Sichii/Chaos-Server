using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

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