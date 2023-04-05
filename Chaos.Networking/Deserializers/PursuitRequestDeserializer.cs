using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="PursuitRequestArgs" />
/// </summary>
public sealed record PursuitRequestDeserializer : ClientPacketDeserializer<PursuitRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.PursuitRequest;

    /// <inheritdoc />
    public override PursuitRequestArgs Deserialize(ref SpanReader reader)
    {
        var gameObjectType = (EntityType)reader.ReadByte();
        var objectId = reader.ReadUInt32();
        var pursuitId = reader.ReadUInt16();

        var args = new List<string>();

        if (reader.Remaining == 1)
        {
            var slotOrLength = reader.ReadByte();

            if (slotOrLength > 0)
                args.Add(slotOrLength.ToString());
        } else
            args = reader.ReadArgs8();

        if (args.Count == 0)
            args = null;

        return new PursuitRequestArgs(
            gameObjectType,
            objectId,
            pursuitId,
            args?.ToArray());
    }
}