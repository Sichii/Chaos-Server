using System.Runtime.InteropServices;
using System.Text;
using Chaos.Extensions.Cryptography;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.ClassMetaData;

public sealed class AbilityMetaData : MetaDataBase<AbilityMetaNode>
{
    public const string MONK_METAFILE_NAME = "SClass5";
    public const string PEASANT_METAFILE_NAME = "SClass0";
    public const string PRIEST_METAFILE_NAME = "SClass4";
    public const string ROGUE_METAFILE_NAME = "SClass2";
    public const string WARRIOR_METAFILE_NAME = "SClass1";
    public const string WIZARD_METAFILE_NAME = "SClass3";
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

        File.WriteAllBytes($"meta/{Name}.dat", buffer.ToArray());

        CheckSum = Crc.Generate32(buffer);
        ZLIB.Compress(ref buffer);
        Data = buffer.ToArray();
    }
}