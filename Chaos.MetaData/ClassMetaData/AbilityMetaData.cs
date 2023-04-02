using System.Runtime.InteropServices;
using System.Text;
using Chaos.Extensions.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

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
        var writer = new SpanWriter(Encoding.GetEncoding(949));

        var nodeCount = Nodes.Count + 4;

        writer.WriteUInt16((ushort)nodeCount);

        var skills = Nodes
                     .Where(node => node.IsSkill)
                     .ToList();

        var spells = Nodes
                     .Where(node => !node.IsSkill)
                     .ToList();

        SKILL_NODE.Serialize(ref writer);

        foreach (var skill in CollectionsMarshal.AsSpan(skills))
            skill.Serialize(ref writer);

        SKILL_END_NODE.Serialize(ref writer);

        SPELL_NODE.Serialize(ref writer);

        foreach (var spell in CollectionsMarshal.AsSpan(spells))
            spell.Serialize(ref writer);

        SPELL_END_NODE.Serialize(ref writer);

        writer.Flush();
        var buffer = writer.ToSpan();

        CheckSum = Crc.Generate32(buffer);
        ZLIB.Compress(ref buffer);
        Data = buffer.ToArray();
    }
}