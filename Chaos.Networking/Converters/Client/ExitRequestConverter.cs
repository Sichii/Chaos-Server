using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ExitRequestArgs" />
/// </summary>
public sealed class ExitRequestConverter : PacketConverterBase<ExitRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ExitRequest;

    /// <inheritdoc />
    public override ExitRequestArgs Deserialize(ref SpanReader reader)
    {
        var isRequest = reader.ReadBoolean();

        return new ExitRequestArgs
        {
            IsRequest = isRequest
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ExitRequestArgs args) => writer.WriteBoolean(args.IsRequest);
}