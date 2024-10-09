using System.Collections.Frozen;
using Chaos.DarkAges.Definitions;
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
                                .GroupBy(node => node.Class)
                                .ToFrozenDictionary(grp => grp.Key, grp => grp.ToList());

        foreach (var nodeGroup in nodesByClass)
        {
            var name = $"SClass{(byte)nodeGroup.Key}";

            var metadata = new AbilityMetaData(name);
            var nodes = nodeGroup.Value;

            // anyone can learn peasant skills
            if (nodeGroup.Key is not BaseClass.Peasant)
                nodes = nodesByClass.TryGetValue(BaseClass.Peasant, out var peasantNodes)
                    ? nodes.Concat(peasantNodes)
                           .ToList()
                    : nodes;

            foreach (var node in nodes)
                metadata.AddNode(node);

            metadata.Compress();

            yield return metadata;
        }
    }
}