#region
using System.Drawing;
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.LightMetaData;
using Chaos.MetaData.NationMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class LightAndNationMetaTests
{
    [Test]
    public void LightPropertyMetaNode_Serialize_Writes_All_Properties()
    {
        var node = new LightPropertyMetaNode("Default")
        {
            EnumValue = 0x0B,
            StartHour = 1,
            EndHour = 2,
            Color = Color.FromArgb(
                10,
                20,
                30,
                40)
        };

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buf = writer.ToSpan()
                        .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buf);

        reader.ReadString8()
              .Should()
              .Be("Default_B");
        reader.ReadInt16();

        reader.ReadString16()
              .Should()
              .Be("1");

        reader.ReadString16()
              .Should()
              .Be("2");

        reader.ReadString16()
              .Should()
              .Be("10");

        reader.ReadString16()
              .Should()
              .Be("20");

        reader.ReadString16()
              .Should()
              .Be("30");

        reader.ReadString16()
              .Should()
              .Be("40");
    }

    [Test]
    public void MapLightMetaNode_Serialize_Writes_Map_And_LightType()
    {
        var node = new MapLightMetaNode(123, "Default");
        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buf = writer.ToSpan()
                        .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buf);

        reader.ReadString8()
              .Should()
              .Be("123");
        reader.ReadInt16();

        reader.ReadString16()
              .Should()
              .Be("Default");
    }

    [Test]
    public void NationDescriptionMetaNode_Serialize_Writes_Name_And_Description()
    {
        var node = new NationDescriptionMetaNode(Nation.Suomi);
        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buf = writer.ToSpan()
                        .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buf);

        reader.ReadString8()
              .Should()
              .StartWith("nation_");

        reader.ReadUInt16()
              .Should()
              .Be(1);

        reader.ReadString16()
              .Should()
              .Be(Nation.Suomi.ToString());
    }
}