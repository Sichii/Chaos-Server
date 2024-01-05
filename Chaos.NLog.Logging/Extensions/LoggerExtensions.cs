using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Chaos.NLog.Logging.Extensions;

/// <summary>
///     Provides extensions for <see cref="ILogger" />.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    ///     Adds metrics to the log event, measuring the time from this call to when Log is called
    /// </summary>
    /// <param name="logger">
    ///     The logger to add metrics to
    /// </param>
    public static ILogger WithMetrics(this ILogger logger)
    {
        if (logger is not LogEvent logEvent)
            logEvent = new LogEvent(logger);

        return logEvent.WithMetrics();
    }

    /// <summary>
    ///     Adds a property to the log event.
    /// </summary>
    /// <param name="logger">
    ///     The logger to add the property to
    /// </param>
    /// <param name="value">
    ///     The value of the property
    /// </param>
    /// <param name="name">
    ///     The name of the property (this will be auto populated to the variable name you pass in if you leave it blank
    /// </param>
    public static ILogger WithProperty(this ILogger logger, object value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        if (logger is not LogEvent logEvent)
            logEvent = new LogEvent(logger);

        return logEvent.WithProperty(name, value);
    }

    /// <summary>
    ///     Adds one or more topics to the log event
    /// </summary>
    /// <param name="logger">
    ///     The logger to add the topics to
    /// </param>
    /// <param name="topics">
    ///     The topics to add to the logger
    /// </param>
    public static ILogger WithTopics(this ILogger logger, params string[] topics)
    {
        if (logger is not LogEvent logEvent)
            logEvent = new LogEvent(logger);

        foreach (var topic in topics)
            logEvent.WithTopic(topic);

        return logEvent;
    }
}