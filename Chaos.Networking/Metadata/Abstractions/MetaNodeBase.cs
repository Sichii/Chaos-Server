using Chaos.IO.Memory;

namespace Chaos.Networking.Metadata.Abstractions;

public abstract record MetaNodeBase(string Name)
{
    public abstract void Serialize(ref SpanWriter writer);
}