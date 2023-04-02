using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.EventMetadata;

/// <summary>
///     Represents a collection of <see cref="EventMetaNode" /> that can be split into sub-sequences
/// </summary>
public sealed class EventMetaNodeCollection : MetaNodeCollection<EventMetaNode>, ISplittingMetaNodeCollection<EventMetaNode>
{
    /// <inheritdoc />
    public IEnumerable<MetaDataBase<EventMetaNode>> Split()
    {
        var nodes = Nodes.GroupBy(node => node.Page);

        foreach (var nodeGroup in nodes)
        {
            var eventMetafile = new EventMetaData(nodeGroup.Key);

            foreach (var node in nodeGroup)
                eventMetafile.AddNode(node);

            eventMetafile.Compress();

            yield return eventMetafile;
        }
    }
}