#region
using System.Text;
using Chaos.IO.Memory;
using Chaos.MetaData.EventMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class EventMetaTests
{
    [Test]
    public void EventMetaData_Compress_Writes_Count_And_Data()
    {
        var coll = new EventMetaNodeCollection();
        coll.AddNode(new EventMetaNode("Q1", 1));
        coll.AddNode(new EventMetaNode("Q2", 1));

        var parts = coll.Split()
                        .ToList();
        var md = parts.Single();

        md.Data
          .Should()
          .NotBeNull();

        md.CheckSum
          .Should()
          .NotBe(0u);
    }

      [Test]
      public void EventMetaNode_Serialize_WithNullProperties_UsesDefaults()
      {
            // All optional properties are null — should use ?? defaults
            var node = new EventMetaNode("QuestNull", 2);

            var writer = new SpanWriter(Encoding.GetEncoding(949));
            node.Serialize(ref writer);
            writer.Flush();

            var buffer = writer.ToSpan()
                               .ToArray();
            var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

            // 02_start
            reader.ReadString8()
                  .Should()
                  .Be("02_start");
            reader.ReadInt16(); // property count

            // 02_title
            reader.ReadString8()
                  .Should()
                  .Be("02_title");

            var titlePropCount = reader.ReadInt16();

            titlePropCount.Should()
                          .Be(1);

            reader.ReadString16()
                  .Should()
                  .Be("QuestNull");

            // 02_id — should be empty string (Id ?? string.Empty)
            reader.ReadString8()
                  .Should()
                  .Be("02_id");

            reader.ReadInt16(); // prop count

            reader.ReadString16()
                  .Should()
                  .Be(string.Empty);

            // 02_qual — should use defaults "1234567" and "012345"
            reader.ReadString8()
                  .Should()
                  .Be("02_qual");

            reader.ReadInt16();

            reader.ReadString16()
                  .Should()
                  .Be("1234567");

            reader.ReadString16()
                  .Should()
                  .Be("012345");
      }

    [Test]
    public void EventMetaNode_Serialize_Writes_Sequence_Of_Subnodes()
    {
        var node = new EventMetaNode("QuestName", 1)
        {
            Id = "q1",
            QualifyingCircles = "123",
            QualifyingClasses = "012",
            Summary = "Sum",
            Result = "Res",
            PrerequisiteEventId = "p0",
            Rewards = "Gold"
        };

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("01_start");
        reader.ReadInt16();

        reader.ReadString8()
              .Should()
              .Be("01_title");
    }
}