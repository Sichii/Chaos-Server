#region
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Utilities;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class HelpersTests
{
    #region ChunkMessage
    [Test]
    public void ChunkMessage_ShortMessage_ShouldReturnSingleChunk()
    {
        var message = "Hello world";

        var result = Helpers.ChunkMessage(message);

        result.Should()
              .HaveCount(1);

        result[0]
            .Should()
            .Be("Hello world");
    }

    [Test]
    public void ChunkMessage_LongMessage_ShouldReturnMultipleChunks()
    {
        var message = new string('a', 134);

        var result = Helpers.ChunkMessage(message);

        result.Should()
              .HaveCount(2);

        result[0]
            .Should()
            .HaveLength(67);

        result[1]
            .Should()
            .HaveLength(67);
    }

    [Test]
    public void ChunkMessage_WithColorCodes_ShouldNotCountColorCodesAsLength()
    {
        // Build a message that is 67 visible chars plus color codes
        // Color code pattern is {=a (3 chars that don't count)
        var message = "{=a" + new string('x', 67);

        var result = Helpers.ChunkMessage(message);

        // The color code adds 3 chars to the string but not to charCount,
        // so the total visible chars are 67 which is exactly MAX_MESSAGE_LINE_LENGTH
        result.Should()
              .HaveCount(1);

        result[0]
            .Should()
            .HaveLength(70); // 3 (color code) + 67 (visible chars)
    }

    [Test]
    public void ChunkMessage_EmptyMessage_ShouldReturnEmptyList()
    {
        var message = string.Empty;

        var result = Helpers.ChunkMessage(message);

        result.Should()
              .BeEmpty();
    }
    #endregion

    #region DefaultChannelMessageHandler
    [Test]
    public void DefaultChannelMessageHandler_ShortMessage_ShouldSendMessageDirectly()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var clientMock = Mock.Get(aisling.Client);
        var message = "Hello";

        Helpers.DefaultChannelMessageHandler(aisling, message);

        clientMock.Verify(c => c.SendDisplayPublicMessage(uint.MaxValue, PublicMessageType.Shout, message), Times.Once);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, message), Times.Once);
    }

    [Test]
    public void DefaultChannelMessageHandler_LongMessage_ShouldSendTruncatedOrangeBarMessage()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var clientMock = Mock.Get(aisling.Client);
        var message = new string('a', 134);

        Helpers.DefaultChannelMessageHandler(aisling, message);

        clientMock.Verify(c => c.SendDisplayPublicMessage(uint.MaxValue, PublicMessageType.Shout, message), Times.Once);

        // The orange bar message should be the first chunk truncated with "..."
        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.EndsWith("..."))), Times.Once);
    }
    #endregion

    #region TryGetMessageColor
    [Test]
    public void TryGetMessageColor_DirectEnumParse_ShouldSucceed()
    {
        var args = new ArgumentCollection(["Gray"]);

        var result = Helpers.TryGetMessageColor(args, out var messageColor);

        result.Should()
              .BeTrue();

        messageColor.Should()
                    .Be(MessageColor.Gray);
    }

    [Test]
    public void TryGetMessageColor_ColorPrefix_ShouldSucceed()
    {
        var args = new ArgumentCollection(["{=a"]);

        var result = Helpers.TryGetMessageColor(args, out var messageColor);

        result.Should()
              .BeTrue();

        messageColor.Should()
                    .Be((MessageColor)(byte)'a');
    }

    [Test]
    public void TryGetMessageColor_SingleLetter_ShouldSucceed()
    {
        var args = new ArgumentCollection(["a"]);

        var result = Helpers.TryGetMessageColor(args, out var messageColor);

        result.Should()
              .BeTrue();

        messageColor.Should()
                    .Be((MessageColor)(byte)'a');
    }

    [Test]
    public void TryGetMessageColor_InvalidString_ShouldReturnFalse()
    {
        var args = new ArgumentCollection(["xyz"]);

        var result = Helpers.TryGetMessageColor(args, out var messageColor);

        result.Should()
              .BeFalse();

        messageColor.Should()
                    .BeNull();
    }

    [Test]
    public void TryGetMessageColor_EmptyArgs_ShouldReturnFalse()
    {
        var args = new ArgumentCollection(Array.Empty<string>());

        var result = Helpers.TryGetMessageColor(args, out var messageColor);

        result.Should()
              .BeFalse();

        messageColor.Should()
                    .BeNull();
    }

    [Test]
    public void TryGetMessageColor_NonLetterSingleChar_ShouldReturnFalse()
    {
        var args = new ArgumentCollection(["!"]);

        var result = Helpers.TryGetMessageColor(args, out var messageColor);

        result.Should()
              .BeFalse();

        messageColor.Should()
                    .BeNull();
    }

    [Test]
    public void TryGetMessageColor_TwoCharString_ShouldReturnFalse()
    {
        var args = new ArgumentCollection(["ab"]);

        var result = Helpers.TryGetMessageColor(args, out var messageColor);

        result.Should()
              .BeFalse();

        messageColor.Should()
                    .BeNull();
    }
    #endregion
}