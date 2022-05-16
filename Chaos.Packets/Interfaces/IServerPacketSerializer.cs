using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Packets.Definitions;

namespace Chaos.Packets.Interfaces;

public interface IServerPacketSerializer
{
    ServerOpCode ServerOpCode { get; }

    void Serialize(ref SpanWriter writer, object args);
}

public interface IServerPacketSerializer<in T> : IServerPacketSerializer
{
    void Serialize(ref SpanWriter writer, T args);
}