#region
using System.Text;
using Chaos.IO.Memory;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class MetaNodeTests
{
    [Test]
    public void Name_IsSetFromConstructor()
    {
        var node = new MetaNode("MyNode");

        node.Name
            .Should()
            .Be("MyNode");
    }

    [Test]
    public void Properties_Count_MatchesAddedItems()
    {
        var node = new MetaNode("CountTest");
        node.Properties.Add("X");
        node.Properties.Add("Y");
        node.Properties.Add("Z");

        node.Properties
            .Count
            .Should()
            .Be(3);
    }

    [Test]
    public void Serialize_EmptyProperties_WritesZeroCount()
    {
        var node = new MetaNode("Empty");

        var writer = new SpanWriter(Encoding.GetEncoding(949));

        node.Serialize(ref writer);

        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();

        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("Empty");

        reader.ReadUInt16()
              .Should()
              .Be(0);
    }

    [Test]
    public void Serialize_Writes_Name_And_Properties()
    {
        var node = new MetaNode("Test");
        node.Properties.Add("A");
        node.Properties.Add("BB");

        var writer = new SpanWriter(Encoding.GetEncoding(949));

        node.Serialize(ref writer);

        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();

        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("Test");

        reader.ReadUInt16()
              .Should()
              .Be(2);

        reader.ReadString16()
              .Should()
              .Be("A");

        reader.ReadString16()
              .Should()
              .Be("BB");
    }
}