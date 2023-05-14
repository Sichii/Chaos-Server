using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.ClassMetaData;

/// <summary>
///     Represents a collection of <see cref="AbilityMetaNode" /> that can be split into sub-sequences
/// </summary>
public sealed class AbilityMetaNodeCollection : MetaNodeCollection<AbilityMetaNode>, ISplittingMetaNodeCollection<AbilityMetaNode>
{
    /// <inheritdoc />
    public IEnumerable<MetaDataBase<AbilityMetaNode>> Split()
    {
        var nodesByClass = Nodes.OrderBy(node => node.Class)
                                .ThenBy(node => node.IsSkill)
                                .ThenBy(node => node.Level)
                                .GroupBy(node => node.Class);

        foreach (var nodeGroup in nodesByClass)
        {
            var name = $"SClass{(byte)nodeGroup.Key}";

            var metadata = new AbilityMetaData(name);

            foreach (var node in nodeGroup)
                metadata.AddNode(node);

            metadata.Compress();

            yield return metadata;
        }
    }
}