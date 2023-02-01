using Chaos.Common.Definitions;
using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.ClassMetaData;

public sealed class AbilityMetaNodeCollection : MetaNodeCollection<AbilityMetaNode>, ISplittingMetaNodeCollection<AbilityMetaNode>
{
    public IEnumerable<MetaDataBase<AbilityMetaNode>> Split()
    {
        var nodesByClass = Nodes.OrderBy(node => node.Class)
                                .ThenBy(node => node.IsSkill)
                                .ThenBy(node => node.Level)
                                .GroupBy(node => node.Class);

        foreach (var nodeGroup in nodesByClass)
        {
            var name = nodeGroup.Key switch
            {
                BaseClass.Peasant => AbilityMetaData.PEASANT_METAFILE_NAME,
                BaseClass.Warrior => AbilityMetaData.WARRIOR_METAFILE_NAME,
                BaseClass.Rogue   => AbilityMetaData.ROGUE_METAFILE_NAME,
                BaseClass.Wizard  => AbilityMetaData.WIZARD_METAFILE_NAME,
                BaseClass.Priest  => AbilityMetaData.PRIEST_METAFILE_NAME,
                BaseClass.Monk    => AbilityMetaData.MONK_METAFILE_NAME,
                _                 => throw new ArgumentOutOfRangeException()
            };

            var metafile = new AbilityMetaData(name);

            foreach (var node in nodeGroup)
                metafile.AddNode(node);

            metafile.Compress();

            yield return metafile;
        }
    }
}