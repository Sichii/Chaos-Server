using Chaos.IO.Memory;

namespace Chaos.MetaData.Abstractions;

/// <summary>
///     Represents a node in the metadata
/// </summary>
public interface IMetaNode
{
    /// <summary>
    ///     The name of the node
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Serializes the node to the writer
    /// </summary>
    void Serialize(ref SpanWriter writer);
}