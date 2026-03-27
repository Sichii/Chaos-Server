#region
using Chaos.NLog.Logging.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class LoggerExtensionsTests
{
    [Test]
    public void WithMetrics_Adds_Metrics_Property_On_Log()
    {
        var baseLogger = new ListLogger();
        var logger = baseLogger.WithMetrics();

        logger.LogInformation("msg");

        baseLogger.Entries
                  .Should()
                  .HaveCount(1);
    }

    [Test]
    public void WithMetrics_WhenAlreadyLogEvent_ShouldReuseExisting()
    {
        // Arrange — first call wraps in LogEvent, second should reuse
        var baseLogger = new ListLogger();
        var logEvent = baseLogger.WithMetrics(); // Creates LogEvent
        var result = logEvent.WithMetrics(); // Should reuse same LogEvent

        // Assert
        result.Should()
              .BeOfType<LogEvent>();

        result.Should()
              .BeSameAs(logEvent);
    }

    [Test]
    public void WithProperty_Adds_Topic_And_Property()
    {
        var baseLogger = new ListLogger();

        var logger = baseLogger.WithProperty(
            new
            {
                A = 1
            },
            "this");

        logger.LogInformation("msg");

        baseLogger.Entries
                  .Should()
                  .HaveCount(1);
    }

    [Test]
    public void WithProperty_WhenAlreadyLogEvent_ShouldReuseExisting()
    {
        var baseLogger = new ListLogger();
        var logEvent = baseLogger.WithMetrics(); // Creates LogEvent
        var result = logEvent.WithProperty("test value");

        result.Should()
              .BeOfType<LogEvent>();

        result.Should()
              .BeSameAs(logEvent);
    }

    [Test]
    public void WithProperty_WithDottedCallerExpression_ShouldExtractLastPart()
    {
        // Arrange
        var baseLogger = new ListLogger();

        var obj = new TestObj
        {
            Name = "test"
        };

        // Act — CallerArgumentExpression captures "obj.Name"
        var result = baseLogger.WithProperty(obj.Name);

        // Assert — should extract "Name" from "obj.Name"
        result.Should()
              .BeOfType<LogEvent>();
        var logEvent = (LogEvent)result;

        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        logEvent.Should()
                .Contain(kvp => kvp.Key == "Name");
    }

    [Test]
    public void WithProperty_WithSimpleName_ShouldNotSplit()
    {
        // Arrange — no dot, so lastDotIndex == -1
        var baseLogger = new ListLogger();
        var testValue = "hello";

        // Act
        var result = baseLogger.WithProperty(testValue);

        // Assert
        result.Should()
              .BeOfType<LogEvent>();
        var logEvent = (LogEvent)result;

        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        logEvent.Should()
                .Contain(kvp => kvp.Key == "TestValue");
    }

    [Test]
    public void WithTopics_ShouldAddAllTopics()
    {
        var baseLogger = new ListLogger();
        var result = baseLogger.WithTopics("topic1", "topic2", "topic3");

        var logEvent = (LogEvent)result;

        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .HaveCount(3);

        topics.Should()
              .Contain("Topic1");

        topics.Should()
              .Contain("Topic2");

        topics.Should()
              .Contain("Topic3");
    }

    [Test]
    public void WithTopics_WhenAlreadyLogEvent_ShouldReuseExisting()
    {
        var baseLogger = new ListLogger();
        var logEvent = baseLogger.WithMetrics(); // Creates LogEvent
        var result = logEvent.WithTopics("topic1", "topic2");

        result.Should()
              .BeSameAs(logEvent);
    }

    [Test]
    public void WithTopics_WhenNotLogEvent_ShouldCreateLogEvent()
    {
        var baseLogger = new ListLogger();
        var result = baseLogger.WithTopics("topic1", "topic2");

        result.Should()
              .BeOfType<LogEvent>();
    }

    private sealed class ListLogger : ILogger
    {
        public List<(LogLevel, string)> Entries { get; } = new();
        public IDisposable? BeginScope<TState>(TState state) where TState: notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }

    private sealed class TestObj
    {
        public string Name { get; init; } = "";
    }
}