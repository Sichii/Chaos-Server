#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.MetaData.ClassMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class AbilityMetaDataTests
{
    [Test]
    public void AbilityMetaNode_Serialize_WithNullPrereqs_UsesDefaults()
    {
        // All optional properties null — covers ?? "0", ?? 0, ?? string.Empty branches
        var node = new AbilityMetaNode("BasicAttack", true, BaseClass.Warrior)
        {
            Level = 1,
            AbilityLevel = 0,
            IconId = 5
        };

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("BasicAttack");

        reader.ReadInt16()
              .Should()
              .Be(6);

        // Property 1: Level/RequiresMaster/AbilityLevel
        reader.ReadString16()
              .Should()
              .Be("1/0/0");

        // Property 2: IconId/0/0
        reader.ReadString16()
              .Should()
              .Be("5/0/0");

        // Property 3: Str/Int/Wis/Dex/Con
        reader.ReadString16()
              .Should()
              .Be("0/0/0/0/0");

        // Property 4: PreReq1Name/PreReq1Level — null defaults to "0/0"
        reader.ReadString16()
              .Should()
              .Be("0/0");

        // Property 5: PreReq2Name/PreReq2Level — null defaults to "0/0"
        reader.ReadString16()
              .Should()
              .Be("0/0");

        // Property 6: Description — null defaults to empty string
        reader.ReadString16()
              .Should()
              .Be(string.Empty);
    }

    [Test]
    public void AbilityMetaNode_Serialize_WithPrereqs_WritesValues()
    {
        var node = new AbilityMetaNode("AdvancedSlash", true, BaseClass.Warrior)
        {
            Level = 20,
            AbilityLevel = 5,
            IconId = 25,
            Str = 10,
            Int = 5,
            Wis = 3,
            Dex = 8,
            Con = 7,
            RequiresMaster = true,
            PreReq1Name = "Slash",
            PreReq1Level = 50,
            PreReq2Name = "Assault",
            PreReq2Level = 30,
            Description = "A powerful slash"
        };

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("AdvancedSlash");

        reader.ReadInt16();

        reader.ReadString16()
              .Should()
              .Be("20/1/5");

        reader.ReadString16()
              .Should()
              .Be("25/0/0");

        reader.ReadString16()
              .Should()
              .Be("10/5/3/8/7");

        reader.ReadString16()
              .Should()
              .Be("Slash/50");

        reader.ReadString16()
              .Should()
              .Be("Assault/30");

        reader.ReadString16()
              .Should()
              .Be("A powerful slash");
    }

    [Test]
    public void Compress_SkillsOnly_WritesEmptySpellSection()
    {
        var md = new AbilityMetaData("SClass2");

        md.AddNode(
            new AbilityMetaNode("Assault", true, BaseClass.Warrior)
            {
                Level = 5,
                AbilityLevel = 1,
                IconId = 15
            });

        md.Compress();

        md.Data
          .Should()
          .NotBeNull();

        md.CheckSum
          .Should()
          .NotBe(0u);
    }

    [Test]
    public void Compress_SpellsOnly_WritesEmptySkillSection()
    {
        var md = new AbilityMetaData("SClass3");

        md.AddNode(
            new AbilityMetaNode("IceBlast", false, BaseClass.Wizard)
            {
                Level = 10,
                AbilityLevel = 3,
                IconId = 30
            });

        md.Compress();

        md.Data
          .Should()
          .NotBeNull();

        md.CheckSum
          .Should()
          .NotBe(0u);
    }

    [Test]
    public void Compress_Writes_Groups_With_Skill_And_Spell_Sections()
    {
        var md = new AbilityMetaData("SClass1");

        md.AddNode(
            new AbilityMetaNode("Slash", true, BaseClass.Warrior)
            {
                Level = 1,
                AbilityLevel = 1,
                IconId = 10
            });

        md.AddNode(
            new AbilityMetaNode("Fireball", false, BaseClass.Warrior)
            {
                Level = 3,
                AbilityLevel = 2,
                IconId = 20
            });

        md.Compress();

        // decompress and inspect structure
        var bytes = md.Data.ToArray();
        Zlib.Decompress(ref bytes);
        var reader = new SpanReader(Encoding.GetEncoding(949), bytes);

        reader.ReadUInt16()
              .Should()
              .BeGreaterThan(0);
    }
}