using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class TimeSpanExtensionsTests
{
    [Fact]
    public void ToReadableString_Should_Not_Show_Days_When_Flag_Set()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            1,
            2,
            3,
            4);

        // Act
        var result = timeSpan.ToReadableString(showDays: false);

        // Assert
        result.Should()
              .Be("2 hours 3 mins 4 secs");
    }

    [Fact]
    public void ToReadableString_Should_Not_Show_Hours_When_Flag_Set()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            2,
            0,
            0,
            0);

        // Act
        var result = timeSpan.ToReadableString(showHours: false);

        // Assert
        result.Should()
              .Be("2 days");
    }

    [Fact]
    public void ToReadableString_Should_Not_Show_Minutes_When_Flag_Set()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            0,
            1,
            30,
            0);

        // Act
        var result = timeSpan.ToReadableString(showMinutes: false);

        // Assert
        result.Should()
              .Be("1 hour");
    }

    [Fact]
    public void ToReadableString_Should_Not_Show_Seconds_When_Flag_Set()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            0,
            0,
            45,
            5);

        // Act
        var result = timeSpan.ToReadableString(showSeconds: false);

        // Assert
        result.Should()
              .Be("45 mins");
    }

    [Fact]
    public void ToReadableString_Should_Return_Empty_String_When_TimeSpan_Is_Zero()
    {
        // Arrange
        var timeSpan = TimeSpan.Zero;

        // Act
        var result = timeSpan.ToReadableString();

        // Assert
        result.Should()
              .Be(string.Empty);
    }

    [Fact]
    public void ToReadableString_Should_Return_Human_Readable_Format_With_Milliseconds()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            0,
            0,
            0,
            10,
            500);

        // Act
        var result = timeSpan.ToReadableString(true);

        // Assert
        result.Should()
              .Be("10 secs 500ms");
    }

    [Fact]
    public void ToReadableString_Should_Return_Human_Readable_Format_Without_Milliseconds()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            2,
            3,
            15,
            20);

        // Act
        var result = timeSpan.ToReadableString();

        // Assert
        result.Should()
              .Be("2 days 3 hours 15 mins 20 secs");
    }
}