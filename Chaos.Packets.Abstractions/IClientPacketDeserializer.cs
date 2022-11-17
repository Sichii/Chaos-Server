using Chaos.IO.Memory;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Packets.Abstractions;

/// <summary>
///     Defines a pattern to deserialize a span of bytes into an object
/// </summary>
public interface IClientPacketDeserializer
{
    /// <summary>
    ///     The opcode associated with this deserializer
    /// </summary>
    ClientOpCode ClientOpCode { get; }

    /// <summary>
    ///     Deserializes a span of bytes into an object
    /// </summary>
    /// <param name="reader">A reference to an object that reads a span of bytes</param>
    object Deserialize(ref SpanReader reader);
}

/// <inheritdoc />
/// <typeparam name="T">A type that inherits from <see cref="Chaos.Packets.Abstractions.IReceiveArgs"/></typeparam>
public interface IClientPacketDeserializer<out T> : IClientPacketDeserializer where T : IReceiveArgs
{
    /// <summary>
    ///     Deserializes a span of bytes into an object
    /// </summary>
    /// <param name="reader">A reference to an object that reads a span of bytes</param>
    new T Deserialize(ref SpanReader reader);
}