using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Deserializers;

public abstract record ClientPacketDeserializer<T> : IClientPacketDeserializer<T> where T: IReceiveArgs
{
    public abstract ClientOpCode ClientOpCode { get; }
    public abstract T Deserialize(ref SpanReader reader);

    object IClientPacketDeserializer.Deserialize(ref SpanReader reader) => Deserialize(ref reader);
}