#region
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
#endregion

namespace Chaos.NLog.Logging.Extensions;

/// <summary>
///     Provides extensions for <see cref="ILogger" />.
/// </summary>
public static class LoggerExtensions
{
    /// <param name="logger">
    ///     The logger to add metrics to
    /// </param>
    extension(ILogger logger)
    {
        /// <summary>
        ///     Adds metrics to the log event, measuring the time from this call to when Log is called
        /// </summary>
        public ILogger WithMetrics()
        {
            if (logger is not LogEvent logEvent)
                logEvent = new LogEvent(logger);

            return logEvent.WithMetrics();
        }

        /// <summary>
        ///     Adds a property to the log event.
        /// </summary>
        /// <param name="value">
        ///     The value of the property
        /// </param>
        /// <param name="name">
        ///     The name of the property (this will be auto populated to the variable name you pass in if you leave it blank
        /// </param>
        public ILogger WithProperty(object value, [CallerArgumentExpression(nameof(value))] string name = "")
        {
            if (logger is not LogEvent logEvent)
                logEvent = new LogEvent(logger);

            var lastDotIndex = name.LastIndexOf('.');

            if (lastDotIndex != -1)
                name = name[(lastDotIndex + 1)..];

            return logEvent.WithProperty(name, value);
        }

        /// <summary>
        ///     Adds one or more topics to the log event
        /// </summary>
        /// <param name="topics">
        ///     The topics to add to the logger
        /// </param>
        public ILogger WithTopics(params ReadOnlySpan<string> topics)
        {
            if (logger is not LogEvent logEvent)
                logEvent = new LogEvent(logger);

            foreach (var topic in topics)
                logEvent.WithTopic(topic!);

            return logEvent;
        }
    }
}