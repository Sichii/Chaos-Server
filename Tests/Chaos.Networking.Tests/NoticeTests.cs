#region
using System.Text;
using Chaos.Networking.Entities;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class NoticeTests
{
    [Test]
    public void Notice_DataShouldBeSmallerOrEqualToOriginal_ForLargeMessages()
    {
        // Zlib compression should compress repeated content effectively
        var longMessage = new string('A', 10000);
        var encoding = Encoding.GetEncoding(949);
        var originalSize = encoding.GetByteCount(longMessage);

        var notice = new Notice(longMessage);

        notice.Data
              .Length
              .Should()
              .BeLessThan(originalSize);
    }

    [Test]
    public void Notice_RecordEquality_DifferentMessage_ShouldNotBeEqual()
    {
        var notice1 = new Notice("Alpha");
        var notice2 = new Notice("Beta");

        notice1.Should()
               .NotBe(notice2);
    }

    [Test]
    public void Notice_RecordEquality_SameMessage_ShouldBeEquivalent()
    {
        var notice1 = new Notice("Test");
        var notice2 = new Notice("Test");

        notice1.CheckSum
               .Should()
               .Be(notice2.CheckSum);

        notice1.Data
               .Should()
               .Equal(notice2.Data);
    }

    [Test]
    public void Notice_ShouldHandleEmptyMessage()
    {
        var notice = new Notice(string.Empty);

        notice.Data
              .Should()
              .NotBeNull();
    }

    [Test]
    public void Notice_ShouldProduceDifferentCheckSum_ForDifferentMessages()
    {
        var notice1 = new Notice("Message A");
        var notice2 = new Notice("Message B");

        notice1.CheckSum
               .Should()
               .NotBe(notice2.CheckSum);
    }

    [Test]
    public void Notice_ShouldProduceDifferentData_ForDifferentMessages()
    {
        var notice1 = new Notice("Message A");
        var notice2 = new Notice("Message B");

        notice1.Data
               .Should()
               .NotEqual(notice2.Data);
    }

    [Test]
    public void Notice_ShouldProduceNonEmptyData()
    {
        var notice = new Notice("Hello, World!");

        notice.Data
              .Should()
              .NotBeEmpty();
    }

    [Test]
    public void Notice_ShouldProduceNonZeroCheckSum()
    {
        var notice = new Notice("Hello, World!");

        notice.CheckSum
              .Should()
              .NotBe(0u);
    }

    [Test]
    public void Notice_ShouldProduceSameCheckSum_ForSameMessage()
    {
        var notice1 = new Notice("Same message");
        var notice2 = new Notice("Same message");

        notice1.CheckSum
               .Should()
               .Be(notice2.CheckSum);
    }

    [Test]
    public void Notice_ShouldProduceSameData_ForSameMessage()
    {
        var notice1 = new Notice("Same message");
        var notice2 = new Notice("Same message");

        notice1.Data
               .Should()
               .Equal(notice2.Data);
    }
}