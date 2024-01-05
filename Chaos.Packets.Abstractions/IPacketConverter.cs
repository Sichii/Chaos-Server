using Chaos.IO.Memory;

namespace Chaos.Packets.Abstractions;

/// <summary>
///     Defines a pattern to deserialize a span of bytes into an object
/// </summary>
public interface IPacketConverter
{
    /// <summary>
    ///     The opcode associated with the converter
    /// </summary>
    byte OpCode { get; }

    /// <summary>
    ///     Deserializes a span of bytes into an object
    /// </summary>
    /// <param name="reader">
    ///     A reference to an object that reads a span of bytes
    /// </param>
    object Deserialize(ref SpanReader reader);

    /// <summary>
    ///     Serializes an object into a span of bytes
    /// </summary>
    /// <param name="writer">
    ///     A reference to an object that write bytes to a span
    /// </param>
    /// <param name="args">
    ///     The object to serialize
    /// </param>
    void Serialize(ref SpanWriter writer, object args);
}

/// <inheritdoc />
/// <typeparam name="T">
///     A type that inherits from <see cref="Chaos.Packets.Abstractions.IPacketSerializable" />
/// </typeparam>
public interface IPacketConverter<T> : IPacketConverter where T: IPacketSerializable
{
    /// <summary>
    ///     Deserializes a span of bytes into an object
    /// </summary>
    /// <param name="reader">
    ///     A reference to an object that reads a span of bytes
    /// </param>
    new T Deserialize(ref SpanReader reader);

    /// <summary>
    ///     Serializes an object into a span of bytes
    /// </summary>
    /// <param name="writer">
    ///     A reference to an object that wrytes bytes to a span
    /// </param>
    /// <param name="args">
    ///     The object to serialize
    /// </param>
    void Serialize(ref SpanWriter writer, T args);
}