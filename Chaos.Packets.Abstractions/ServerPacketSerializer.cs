using Chaos.IO.Memory;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Packets.Abstractions;

/// <inheritdoc />
public abstract record ServerPacketSerializer<T> : IServerPacketSerializer<T> where T: ISendArgs
{
    /// <inheritdoc />
    public abstract ServerOpCode ServerOpCode { get; }

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer, object args)
    {
        if (args is T tArgs)
            Serialize(ref writer, tArgs);
    }

    /// <inheritdoc />
    public abstract void Serialize(ref SpanWriter writer, T args);
}