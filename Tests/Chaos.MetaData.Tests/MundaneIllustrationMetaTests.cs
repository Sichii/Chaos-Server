#region
using System.Text;
using Chaos.IO.Memory;
using Chaos.MetaData.MundaneMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class MundaneIllustrationMetaTests
{
    [Test]
    public void MundaneIllustrationMetaData_Compress_MultipleNodes_AllAreEncoded()
    {
        var metaData = new MundaneIllustrationMetaData();
        metaData.AddNode(new MundaneIllustrationMetaNode("Guard", "guard.epf"));
        metaData.AddNode(new MundaneIllustrationMetaNode("Merchant", "merchant.epf"));
        metaData.Compress();

        metaData.Data
                .Should()
                .NotBeEmpty();

        metaData.CheckSum
                .Should()
                .NotBe(0u);
    }

    [Test]
    public void MundaneIllustrationMetaData_Compress_ProducesNonEmptyData()
    {
        var metaData = new MundaneIllustrationMetaData();
        metaData.AddNode(new MundaneIllustrationMetaNode("Guard", "guard.epf"));
        metaData.Compress();

        metaData.CheckSum
                .Should()
                .NotBe(0u);

        metaData.Data
                .Should()
                .NotBeEmpty();
    }

    [Test]
    public void MundaneIllustrationMetaData_Name_IsNPCIllust()
    {
        var metaData = new MundaneIllustrationMetaData();

        metaData.Name
                .Should()
                .Be("NPCIllust");
    }

    [Test]
    public void MundaneIllustrationMetaNode_Serialize_Writes_Name_And_ImageName()
    {
        var node = new MundaneIllustrationMetaNode("Guard", "guard.epf");
        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buf = writer.ToSpan()
                        .ToArray();

        var reader = new SpanReader(Encoding.GetEncoding(949), buf);

        reader.ReadString8()
              .Should()
              .Be("Guard");

        reader.ReadUInt16()
              .Should()
              .Be(1); // 1 property

        reader.ReadString16()
              .Should()
              .Be("guard.epf");
    }
}