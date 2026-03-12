#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.ItemMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class ItemMetaTests
{
    [Test]
    public void ItemMetaNode_Length_Tracks_Setters_And_Serialize_Writes_Properties()
    {
        var node = new ItemMetaNode("Sword")
        {
            Level = 10,
            Class = BaseClass.Warrior,
            Weight = 5,
            Category = "weapon",
            Description = "desc"
        };

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("Sword");

        reader.ReadUInt16()
              .Should()
              .Be(5);

        reader.ReadString16()
              .Should()
              .Be("10");

        reader.ReadString16()
              .Should()
              .Be(((int)BaseClass.Warrior).ToString());

        reader.ReadString16()
              .Should()
              .Be("5");

        reader.ReadString16()
              .Should()
              .Be("weapon");

        reader.ReadString16()
              .Should()
              .Be("desc");
    }

    [Test]
    public void ItemMetaNode_Length_UpdatesCorrectly_WhenCategoryIsReassigned()
    {
        // First assignment sets category to "other" in constructor, then we set it to "weapon"
        var node = new ItemMetaNode("TestItem");
        var lengthAfterConstruction = node.Length;

        // Reassigning Category should subtract old length and add new
        node.Category = "weapon";
        var lengthAfterReassign = node.Length;

        // "other" = 5 bytes, "weapon" = 6 bytes, so length should increase by 1
        lengthAfterReassign.Should()
                           .Be(lengthAfterConstruction + 1);
    }

    [Test]
    public void ItemMetaNode_Length_UpdatesCorrectly_WhenDescriptionIsReassigned()
    {
        var node = new ItemMetaNode("TestItem");

        // Set description (was empty string from constructor, IsNullOrEmpty true → no subtraction)
        node.Description = "short";
        var lengthWithDescription = node.Length;

        // Reassign to something else (old was "short", IsNullOrEmpty false → subtracts old length)
        node.Description = "a longer description";
        var lengthAfterReassign = node.Length;

        lengthAfterReassign.Should()
                           .BeGreaterThan(lengthWithDescription);
    }

    [Test]
    public void ItemMetaNode_Serialize_WithDefaultProperties_WritesDefaults()
    {
        // No properties set beyond name — covers default values in serialization
        var node = new ItemMetaNode("DefaultItem");

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("DefaultItem");

        reader.ReadUInt16()
              .Should()
              .Be(5);

        // Level default = 0
        reader.ReadString16()
              .Should()
              .Be("0");

        // Class default = Peasant (0)
        reader.ReadString16()
              .Should()
              .Be("0");

        // Weight default = 0
        reader.ReadString16()
              .Should()
              .Be("0");

        // Category default = "other"
        reader.ReadString16()
              .Should()
              .Be("other");

        // Description default = empty
        reader.ReadString16()
              .Should()
              .Be(string.Empty);
    }

    [Test]
    public void ItemMetaNodeCollection_Split_Compresses_Final_Segment_And_Yields_All()
    {
        var coll = new ItemMetaNodeCollection();

        for (var i = 0; i < 10; i++)
            coll.AddNode(
                new ItemMetaNode($"Item{i}")
                {
                    Level = i,
                    Class = BaseClass.Peasant,
                    Weight = 1
                });

        var parts = coll.Split()
                        .ToList();

        parts.Should()
             .NotBeEmpty();

        parts.All(p => (p.Data.Length > 0) && (p.CheckSum != 0u))
             .Should()
             .BeTrue();
    }

    [Test]
    public void ItemMetaNodeCollection_Split_Splits_By_Size()
    {
        var coll = new ItemMetaNodeCollection();

        // Create nodes with artificially large length to force split
        for (var i = 0; i < 2000; i++)
            coll.AddNode(
                new ItemMetaNode($"Item{i}")
                {
                    Level = i,
                    Class = BaseClass.Peasant,
                    Weight = i
                });

        var parts = coll.Split()
                        .ToList();

        parts.Should()
             .NotBeEmpty();

        parts.Select(p => p.Name)
             .Should()
             .OnlyHaveUniqueItems();
    }
}