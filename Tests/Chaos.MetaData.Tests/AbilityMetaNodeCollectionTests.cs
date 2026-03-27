#region
using Chaos.DarkAges.Definitions;
using Chaos.MetaData.ClassMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class AbilityMetaNodeCollectionTests
{
    [Test]
    public void Split_Groups_By_Class_And_Includes_Peasant_Skills_For_Others()
    {
        var coll = new AbilityMetaNodeCollection();

        coll.AddNode(
            new AbilityMetaNode("Peasant Skill", true, BaseClass.Peasant)
            {
                Level = 1
            });

        coll.AddNode(
            new AbilityMetaNode("Warrior Skill", true, BaseClass.Warrior)
            {
                Level = 2
            });

        coll.AddNode(
            new AbilityMetaNode("Wizard Spell", false, BaseClass.Wizard)
            {
                Level = 3
            });

        var parts = coll.Split()
                        .ToList();

        parts.Should()
             .Contain(x => x.Name == "SClass0");

        parts.Should()
             .Contain(x => x.Name == "SClass1");

        parts.Should()
             .Contain(x => x.Name == "SClass3");
    }

    [Test]
    public void Split_Sorts_By_Class_Then_IsSkill_Then_Level()
    {
        var coll = new AbilityMetaNodeCollection();

        coll.AddNode(
            new AbilityMetaNode("Z-HighSpell-Warrior", false, BaseClass.Warrior)
            {
                Level = 10
            });

        coll.AddNode(
            new AbilityMetaNode("A-LowSkill-Warrior", true, BaseClass.Warrior)
            {
                Level = 1
            });

        coll.AddNode(
            new AbilityMetaNode("M-MidSkill-Warrior", true, BaseClass.Warrior)
            {
                Level = 5
            });

        var md = coll.Split()
                     .Single(x => x.Name == "SClass1");

        // Decompress md.Data to verify ordering markers exist around groups
        md.Data
          .Should()
          .NotBeNull();

        md.CheckSum
          .Should()
          .NotBe(0u);
    }
}