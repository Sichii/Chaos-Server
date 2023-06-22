using Chaos.IO.Memory;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Packets.Abstractions;

/// <inheritdoc />
public abstract record ClientPacketDeserializer<T> : IClientPacketDeserializer<T> where T: IReceiveArgs
{
    /// <inheritdoc />
    public abstract ClientOpCode ClientOpCode { get; }

    /// <inheritdoc />
    object IClientPacketDeserializer.Deserialize(ref SpanReader reader) => Deserialize(ref reader);

    /// <inheritdoc />
    public abstract T Deserialize(ref SpanReader reader);
}