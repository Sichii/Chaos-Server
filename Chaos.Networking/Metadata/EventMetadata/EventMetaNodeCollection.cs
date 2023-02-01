using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.EventMetadata;

public sealed class EventMetaNodeCollection : MetaNodeCollection<EventMetaNode>
{
    public IEnumerable<EventMetaData> Split()
    {
        var nodes = Nodes.GroupBy(node => node.Circle);

        foreach (var nodeGroup in nodes)
        {
            var eventMetafile = new EventMetaData(nodeGroup.Key);
            var index = 1;

            foreach (var node in nodeGroup)
            {
                node.Sequence = index++;
                eventMetafile.AddNode(node);
            }

            eventMetafile.Compress();

            yield return eventMetafile;
        }
    }
}