using Chaos.IO.Memory;
using Chaos.Packets.Definitions;

namespace Chaos.Packets.Interfaces;

public interface IClientPacketDeserializer
{
    ClientOpCode ClientOpCode { get; }

    object Deserialize(ref SpanReader reader);
}

public interface IClientPacketDeserializer<out T> : IClientPacketDeserializer
{
    new T Deserialize(ref SpanReader reader);
}