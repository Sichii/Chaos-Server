using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="BoardRequestArgs" />
/// </summary>
public sealed record BoardRequestDeserializer : ClientPacketDeserializer<BoardRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.BoardRequest;

    /// <inheritdoc />
    public override BoardRequestArgs Deserialize(ref SpanReader reader)
    {
        var boardRequestType = (BoardRequestType)reader.ReadByte();

        return new BoardRequestArgs(boardRequestType);
    }
}