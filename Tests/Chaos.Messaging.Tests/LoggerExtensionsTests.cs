#region
using Chaos.NLog.Logging.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
#endregion

namespace Chaos.Messaging.Tests;

public sealed class LoggerExtensionsTests
{
    [Test]
    public void WithProperty_And_WithTopics_Do_Not_Throw_And_Produce_Log()
    {
        var baseLogger = new CapturingLogger();

        var logger = baseLogger.WithProperty(
                                   new
                                   {
                                       Name = "X"
                                   },
                                   "this")
                               .WithTopics("alpha", "beta");

        logger.LogInformation("hello");

        baseLogger.States
                  .Count
                  .Should()
                  .Be(1);
    }

    private sealed class CapturingLogger : ILogger
    {
        public List<object> States { get; } = new();
        public IDisposable? BeginScope<TState>(TState state) where TState: notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => States.Add(state!);
    }
}