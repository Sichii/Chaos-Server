using Chaos.IO.Memory;
using Chaos.Packets.Definitions;

namespace Chaos.Packets.Abstractions;

public interface IServerPacketSerializer
{
    ServerOpCode ServerOpCode { get; }

    void Serialize(ref SpanWriter writer, object args);
}

public interface IServerPacketSerializer<in T> : IServerPacketSerializer
{
    void Serialize(ref SpanWriter writer, T args);
}