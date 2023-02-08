using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.EventMetadata;

public sealed class EventMetaNodeCollection : MetaNodeCollection<EventMetaNode>
{
    public IEnumerable<EventMetaData> Split()
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