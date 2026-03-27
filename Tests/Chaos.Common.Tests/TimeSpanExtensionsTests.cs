#region
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Common.Tests;

public sealed class TimeSpanExtensionsTests
{
    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
    public void ToReadableString_Should_Omit_Components_That_Are_Zero()
    {
        // Only minutes set — days, hours, seconds are 0
        var timeSpan = new TimeSpan(
            0,
            0,
            5,
            0);

        var result = timeSpan.ToReadableString();

        result.Should()
              .Be("5 mins");
    }

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
    public void ToReadableString_Should_Show_ZeroMilliseconds_AsEmpty_WhenShowMilliseconds()
    {
        // 0 milliseconds with showMilliseconds=true
        var timeSpan = new TimeSpan(
            0,
            0,
            0,
            5,
            0);

        var result = timeSpan.ToReadableString(true);

        result.Should()
              .Be("5 secs");
    }

    [Test]
    public void ToReadableString_Should_Use_Singular_Day_When_DayIs1()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            1,
            0,
            0,
            0);

        // Act
        var result = timeSpan.ToReadableString();

        // Assert
        result.Should()
              .Be("1 day");
    }

    [Test]
    public void ToReadableString_Should_Use_Singular_Hour_When_HourIs1()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            0,
            1,
            0,
            0);

        // Act
        var result = timeSpan.ToReadableString();

        // Assert
        result.Should()
              .Be("1 hour");
    }

    [Test]
    public void ToReadableString_Should_Use_Singular_Min_When_MinuteIs1()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            0,
            0,
            1,
            0);

        // Act
        var result = timeSpan.ToReadableString();

        // Assert
        result.Should()
              .Be("1 min");
    }

    [Test]
    public void ToReadableString_Should_Use_Singular_Sec_When_SecondIs1()
    {
        // Arrange
        var timeSpan = new TimeSpan(
            0,
            0,
            0,
            1);

        // Act
        var result = timeSpan.ToReadableString();

        // Assert
        result.Should()
              .Be("1 sec");
    }
}