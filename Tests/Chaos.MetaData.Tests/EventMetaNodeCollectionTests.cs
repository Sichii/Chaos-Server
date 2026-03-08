#region
using Chaos.MetaData.EventMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class EventMetaNodeCollectionTests
{
    [Test]
    public void Split_CompressedData_HasNonNullData()
    {
        var collection = new EventMetaNodeCollection();

        collection.Nodes.Add(
            new EventMetaNode("Quest1", 1)
            {
                Summary = "Test"
            });

        var result = collection.Split()
                               .ToList();

        result[0]
            .Data
            .Should()
            .NotBeNull();

        result[0]
            .Data
            .Length
            .Should()
            .BeGreaterThan(0);
    }

    [Test]
    public void Split_CompressedData_HasNonZeroCheckSum()
    {
        var collection = new EventMetaNodeCollection();

        collection.Nodes.Add(
            new EventMetaNode("Quest1", 1)
            {
                Summary = "Test"
            });

        var result = collection.Split()
                               .ToList();

        result[0]
            .CheckSum
            .Should()
            .NotBe(0u);
    }

    [Test]
    public void Split_EmptyCollection_YieldsNothing()
    {
        var collection = new EventMetaNodeCollection();

        var result = collection.Split()
                               .ToList();

        result.Should()
              .BeEmpty();
    }

    [Test]
    public void Split_MultiplePages_YieldsOnePerPage()
    {
        var collection = new EventMetaNodeCollection();

        collection.Nodes.Add(
            new EventMetaNode("Quest1", 1)
            {
                Summary = "Page 1 quest"
            });

        collection.Nodes.Add(
            new EventMetaNode("Quest2", 2)
            {
                Summary = "Page 2 quest"
            });

        collection.Nodes.Add(
            new EventMetaNode("Quest3", 1)
            {
                Summary = "Another page 1 quest"
            });

        var result = collection.Split()
                               .ToList();

        result.Should()
              .HaveCount(2);
    }

    [Test]
    public void Split_SinglePage_YieldsOneMetaData()
    {
        var collection = new EventMetaNodeCollection();

        collection.Nodes.Add(
            new EventMetaNode("Quest1", 1)
            {
                Summary = "A quest"
            });

        var result = collection.Split()
                               .ToList();

        result.Should()
              .HaveCount(1);
    }
}