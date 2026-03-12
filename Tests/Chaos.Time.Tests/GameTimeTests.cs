#region
using Chaos.DarkAges.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public class GameTimeTests
{
    private static readonly DateTime TestOrigin = new(
        2022,
        11,
        1,
        0,
        0,
        0,
        DateTimeKind.Utc);

    [Test]
    public void Addition_Operator_Should_Add_TimeSpan()
    {
        // Arrange
        var gameTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));
        var timeSpan = TimeSpan.FromHours(2);

        // Act
        var result = gameTime + timeSpan;

        // Assert
        result.Hour
              .Should()
              .Be(12);
    }

    [Test]
    public void CompareTo_Should_Return_Correct_Values()
    {
        // Arrange
        var earlierTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var laterTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                12,
                0,
                0));

        var sameTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        // Act & Assert
        earlierTime.CompareTo(laterTime)
                   .Should()
                   .BeLessThan(0);

        laterTime.CompareTo(earlierTime)
                 .Should()
                 .BeGreaterThan(0);

        earlierTime.CompareTo(sameTime)
                   .Should()
                   .Be(0);
    }

    [Test]
    public void CompareTo_With_GameTime_Object_Should_Compare_Correctly()
    {
        var gameTime1 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var gameTime2 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                12,
                0,
                0));

        var result = gameTime1.CompareTo((object)gameTime2);

        result.Should()
              .BeLessThan(0);
    }

    [Test]
    public void CompareTo_With_Null_Should_Return_1()
    {
        // Arrange
        var gameTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        // Act
        var result = gameTime.CompareTo(null);

        // Assert
        result.Should()
              .Be(1);
    }

    [Test]
    public void CompareTo_With_Wrong_Type_Should_Throw_Exception()
    {
        // Arrange
        var gameTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        // Act & Assert
        var act = () => gameTime.CompareTo("not a gametime");

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void Comparison_Operators_Should_Work_Correctly()
    {
        // Arrange
        var earlierTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var laterTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                12,
                0,
                0));

        // Act & Assert
        (laterTime > earlierTime).Should()
                                 .BeTrue();

        (laterTime >= earlierTime).Should()
                                  .BeTrue();

        (earlierTime < laterTime).Should()
                                 .BeTrue();

        (earlierTime <= laterTime).Should()
                                  .BeTrue();

        // ReSharper disable once EqualExpressionComparison
        (earlierTime >= earlierTime).Should()
                                    .BeTrue();

        // ReSharper disable once EqualExpressionComparison
        (laterTime <= laterTime).Should()
                                .BeTrue();
    }

    [Test]
    public void Constructor_With_DateTime_Should_Create_GameTime()
    {
        // Arrange
        var dateTime = new DateTime(
            2023,
            5,
            15,
            12,
            30,
            45);

        // Act
        var gameTime = new GameTime(dateTime);

        // Assert
        gameTime.Year
                .Should()
                .Be(2023);

        gameTime.Month
                .Should()
                .Be(5);

        gameTime.Day
                .Should()
                .Be(15);

        gameTime.Hour
                .Should()
                .Be(12);

        gameTime.Minute
                .Should()
                .Be(30);
    }

    [Test]
    public void Constructor_With_Ticks_Should_Create_GameTime()
    {
        // Arrange
        var ticks = 1000000L;

        // Act
        var gameTime = new GameTime(ticks);

        // Assert
        gameTime.Ticks
                .Should()
                .Be(ticks);
    }

    [Test]
    public void Equality_Operators_Should_Work_Correctly()
    {
        // Arrange
        var gameTime1 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var gameTime2 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var gameTime3 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                11,
                0,
                0));

        // Act & Assert
        (gameTime1 == gameTime2).Should()
                                .BeTrue();

        (gameTime1 != gameTime3).Should()
                                .BeTrue();

        (gameTime1 == gameTime3).Should()
                                .BeFalse();

        (gameTime1 != gameTime2).Should()
                                .BeFalse();
    }

    [Test]
    public void Equals_Should_Work_Correctly()
    {
        // Arrange
        var gameTime1 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var gameTime2 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var gameTime3 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                11,
                0,
                0));

        // Act & Assert
        gameTime1.Equals(gameTime2)
                 .Should()
                 .BeTrue();

        gameTime1.Equals(gameTime3)
                 .Should()
                 .BeFalse();

        gameTime1.Equals((object)gameTime2)
                 .Should()
                 .BeTrue();

        gameTime1.Equals((object)gameTime3)
                 .Should()
                 .BeFalse();

        gameTime1.Equals("not a gametime")
                 .Should()
                 .BeFalse();

        gameTime1.Equals(null)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void FromDateTime_Should_Convert_Correctly()
    {
        // Arrange
        var realDateTime = TestOrigin.AddHours(1); // 1 hour after origin

        // Act
        var gameTime = GameTime.FromDateTime(realDateTime);

        // Assert
        // 1 real hour at 24x speed = 24 game hours = exactly 1 day from epoch
        var expectedTicks = TimeSpan.FromHours(1)
                                    .Ticks
                            * 24;

        gameTime.Ticks
                .Should()
                .Be(expectedTicks);
    }

    [Test]
    public void GameTime_Speed_Multiplier_Should_Be_24x()
    {
        // Arrange - 1 hour in real time
        var realStartTime = TestOrigin;
        var realEndTime = realStartTime.AddHours(1);

        // Act
        var gameStartTime = GameTime.FromDateTime(realStartTime);
        var gameEndTime = GameTime.FromDateTime(realEndTime);

        // Assert
        var gameTimeDifference = gameEndTime - gameStartTime;

        gameTimeDifference.TotalHours
                          .Should()
                          .BeApproximately(24.0, 0.1); // 1 hour real = 24 hours game
    }

    [Test]
    public void GetDaySuffix_Should_Return_Correct_Suffixes()
    {
        var testCases = new[]
        {
            (1, "st"),
            (2, "nd"),
            (3, "rd"),
            (4, "th"),
            (11, "th"), // Special case
            (12, "th"), // Special case
            (13, "th"), // Special case
            (21, "st"),
            (22, "nd"),
            (23, "rd"),
            (31, "st")
        };

        foreach ((var day, var expectedSuffix) in testCases)
        {
            // Arrange
            var gameTime = new GameTime(
                new DateTime(
                    2023,
                    1,
                    day,
                    10,
                    0,
                    0));

            // Act
            var result = gameTime.ToString();

            // Assert
            result.Should()
                  .Contain($"{day}{expectedSuffix}", $"Day {day} should have suffix '{expectedSuffix}'");
        }
    }

    [Test]
    public void GetHashCode_Should_Return_Same_For_Equal_GameTimes()
    {
        // Arrange
        var gameTime1 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var gameTime2 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        // Act & Assert
        gameTime1.GetHashCode()
                 .Should()
                 .Be(gameTime2.GetHashCode());
    }

    [Test]
    public void Now_Should_Return_Current_Game_Time()
    {
        // Act
        var now = GameTime.Now;

        // Assert
        now.Should()
           .NotBeNull();

        // GameTime starts from origin in 2022, so year should be reasonable
        now.Year
           .Should()
           .BeGreaterThan(0);
    }

    [Test]
    public void Properties_Should_Return_Correct_DateTime_Components()
    {
        // Arrange
        var dateTime = new DateTime(
            2023,
            7,
            23,
            14,
            45,
            30);
        var gameTime = new GameTime(dateTime);

        // Act & Assert
        gameTime.Year
                .Should()
                .Be(2023);

        gameTime.Month
                .Should()
                .Be(7);

        gameTime.Day
                .Should()
                .Be(23);

        gameTime.Hour
                .Should()
                .Be(14);

        gameTime.Minute
                .Should()
                .Be(45);

        gameTime.Ticks
                .Should()
                .Be(dateTime.Ticks);
    }

    [Test]
    public void Subtraction_Operator_Should_Return_TimeSpan_Between_GameTimes()
    {
        // Arrange
        var gameTime1 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));

        var gameTime2 = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                12,
                0,
                0));

        // Act
        var result = gameTime2 - gameTime1;

        // Assert
        result.Should()
              .Be(TimeSpan.FromHours(2));
    }

    [Test]
    public void Subtraction_Operator_Should_Subtract_TimeSpan()
    {
        // Arrange
        var gameTime = new GameTime(
            new DateTime(
                2023,
                1,
                1,
                10,
                0,
                0));
        var timeSpan = TimeSpan.FromHours(3);

        // Act
        var result = gameTime - timeSpan;

        // Assert
        result.Hour
              .Should()
              .Be(7);
    }

    [Test]
    public void TimeOfDay_Should_Return_Correct_Light_Level()
    {
        // Test different hours and their expected light levels
        // Include boundary values to exercise all switch branches
        var testCases = new[]
        {
            (10, LightLevel.Lightest_A), // Start of lightest
            (12, LightLevel.Lightest_A), // Noon - brightest
            (17, LightLevel.Lightest_A), // End of lightest
            (9, LightLevel.Lighter_A), // Morning
            (18, LightLevel.Lighter_A), // Evening boundary
            (8, LightLevel.Light_A), // Early morning
            (19, LightLevel.Light_A), // Evening boundary
            (7, LightLevel.Dark_A), // Dawn
            (20, LightLevel.Dark_A), // Dusk
            (6, LightLevel.Darker_A), // Pre-dawn
            (21, LightLevel.Darker_A), // Post-dusk
            (0, LightLevel.Darkest_A), // Midnight
            (3, LightLevel.Darkest_A), // Night - darkest
            (5, LightLevel.Darkest_A), // Early hours
            (22, LightLevel.Darkest_A), // Late night
            (23, LightLevel.Darkest_A) // Near midnight
        };

        foreach ((var hour, var expectedLight) in testCases)
        {
            // Arrange
            var gameTime = new GameTime(
                new DateTime(
                    2023,
                    1,
                    1,
                    hour,
                    0,
                    0));

            // Act
            var lightLevel = gameTime.TimeOfDay;

            // Assert
            lightLevel.Should()
                      .Be(expectedLight, $"Hour {hour} should have light level {expectedLight}");
        }
    }

    [Test]
    public void ToDateTime_Should_Convert_Back_Correctly()
    {
        // Arrange
        var originalDateTime = DateTime.UtcNow;
        var gameTime = GameTime.FromDateTime(originalDateTime);

        // Act
        var convertedBack = gameTime.ToDateTime();

        // Assert
        // Should be close to original (within some tolerance for calculation precision)
        var difference = Math.Abs((convertedBack - originalDateTime).TotalMilliseconds);

        difference.Should()
                  .BeLessThan(1000); // Within 1 second tolerance
    }

    [Test]
    public void ToString_With_Format_Should_Apply_Format()
    {
        // Arrange
        var gameTime = new GameTime(
            new DateTime(
                2023,
                5,
                3,
                10,
                0,
                0));
        var format = "yyyy-MM-dd";

        // Act
        var result = gameTime.ToString(format);

        // Assert
        result.Should()
              .Contain("Year");

        result.Should()
              .Contain("2023-05-03");

        result.Should()
              .Contain("rd"); // Day suffix
    }

    [Test]
    public void ToString_Without_Format_Should_Return_Default_Format()
    {
        // Arrange
        var gameTime = new GameTime(
            new DateTime(
                2023,
                5,
                3,
                10,
                0,
                0));

        // Act
        var result = gameTime.ToString();

        // Assert
        result.Should()
              .Contain("Year");

        result.Should()
              .Contain("May 3rd");
    }
}