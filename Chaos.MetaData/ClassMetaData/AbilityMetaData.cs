#region
using System.Text;
using Chaos.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;
#endregion

namespace Chaos.MetaData.ClassMetaData;

/// <summary>
///     Represents a compressible collection of <see cref="AbilityMetaNode" />
/// </summary>
public sealed class AbilityMetaData : MetaDataBase<AbilityMetaNode>
{
    private static readonly MetaNode SKILL_END_NODE = new("Skill_End");
    private static readonly MetaNode SKILL_NODE = new("Skill");
    private static readonly MetaNode SPELL_END_NODE = new("Spell_End");
    private static readonly MetaNode SPELL_NODE = new("Spell");

    /// <inheritdoc />
    public AbilityMetaData(string name)
        : base(name) { }

    /// <inheritdoc />
    public override void Compress()
    {
        var writer = new SpanWriter(Encoding.GetEncoding(949), usePooling: true);
        using var disposable = writer;

        var nodeCount = Nodes.Count + 4;

        writer.WriteUInt16((ushort)nodeCount);

        var skills = Nodes.Where(node => node.IsSkill)
                          .ToArray();

        var spells = Nodes.Where(node => !node.IsSkill)
                          .ToArray();

        SKILL_NODE.Serialize(ref writer);

        foreach (var skill in skills.AsSpan())
            skill.Serialize(ref writer);

        SKILL_END_NODE.Serialize(ref writer);

        SPELL_NODE.Serialize(ref writer);

        foreach (var spell in spells.AsSpan())
            spell.Serialize(ref writer);

        SPELL_END_NODE.Serialize(ref writer);

        writer.Flush();
        var buffer = writer.ToSpan();

        CheckSum = Crc.Generate32(buffer);
        Zlib.Compress(ref buffer);
        Data = buffer.ToArray();
    }
}