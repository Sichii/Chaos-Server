using Chaos.IO.Memory;

namespace Chaos.Packets.Abstractions;

/// <summary>
///     A base packet converter that forwards non-generic methods to the associated generic methods
/// </summary>
/// <typeparam name="T">
///     The serializable type the converter is for
/// </typeparam>
public abstract class PacketConverterBase<T> : IPacketConverter<T> where T: IPacketSerializable
{
    /// <inheritdoc />
    public abstract byte OpCode { get; }

    /// <inheritdoc />
    object IPacketConverter.Deserialize(ref SpanReader reader) => Deserialize(ref reader);

    /// <inheritdoc />
    public abstract T Deserialize(ref SpanReader reader);

    /// <inheritdoc />
    public abstract void Serialize(ref SpanWriter writer, T args);

    /// <inheritdoc />
    void IPacketConverter.Serialize(ref SpanWriter writer, object args) => Serialize(ref writer, (T)args);
}