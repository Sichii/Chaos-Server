using Chaos.IO.Memory;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Packets.Abstractions;

/// <summary>
///     Defines a pattern to serialize an object to a span of bytes
/// </summary>
public interface IServerPacketSerializer
{
    /// <summary>
    ///     The opcode associated with this serializer
    /// </summary>
    ServerOpCode ServerOpCode { get; }

    /// <summary>
    ///     Serializes an object into a span of bytes
    /// </summary>
    /// <param name="writer">A reference to an object that write bytes to a span</param>
    /// <param name="args">The object to serialize</param>
    void Serialize(ref SpanWriter writer, object args);
}

/// <inheritdoc />
/// <typeparam name="T">A type that inherits from <see cref="Chaos.Packets.Abstractions.ISendArgs"/></typeparam>
public interface IServerPacketSerializer<in T> : IServerPacketSerializer where T : ISendArgs
{
    /// <summary>
    ///     Serializes an object into a span of bytes
    /// </summary>
    /// <param name="writer">A reference to an object that wrytes bytes to a span</param>
    /// <param name="args">The object to serialize</param>
    void Serialize(ref SpanWriter writer, T args);
}