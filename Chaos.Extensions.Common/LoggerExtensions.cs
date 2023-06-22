using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extensions for <see cref="ILogger" />.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    ///     Adds a property to the log event.
    /// </summary>
    /// <param name="logger">The logger to add the property to</param>
    /// <param name="value">The value of the property</param>
    /// <param name="name">
    ///     The name of the property (this will be auto populated to the variable name you pass in if you leave
    ///     it blank
    /// </param>
    public static ILogger WithProperty(this ILogger logger, object value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        if (logger is not LogEvent logEvent)
            logEvent = new LogEvent(logger);

        if (name.EqualsI("this"))
            name = value.GetType().Name;

        if (!char.IsUpper(name[0]))
            name = name.FirstUpper();

        return logEvent.WithProperty(name, value);
    }
}

internal sealed class LogEvent : ILogger, IReadOnlyList<KeyValuePair<string, object>>
{
    private readonly IList<KeyValuePair<string, object>> ExtraProperties;
    private readonly ILogger Logger;
    private IReadOnlyList<KeyValuePair<string, object>> LogValues = null!;

    /// <inheritdoc />
    public int Count => LogValues.Count + ExtraProperties.Count;

    public LogEvent(ILogger logger)
    {
        Logger = logger;
        ExtraProperties = new List<KeyValuePair<string, object>>();
    }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState: notnull => Logger.BeginScope(state);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => LogValues.Concat(ExtraProperties).GetEnumerator();

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => Logger.IsEnabled(logLevel);

    /// <inheritdoc />
    public KeyValuePair<string, object> this[int index]
    {
        get
        {
            if (index < LogValues.Count)
                return LogValues[index];

            return ExtraProperties[index - LogValues.Count];
        }
    }

    /// <inheritdoc />
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        LogValues = state as IReadOnlyList<KeyValuePair<string, object>> ?? new List<KeyValuePair<string, object>>();

        Logger.Log(
            logLevel,
            eventId,
            this,
            exception,
            (_, e) => formatter(state, e));
    }

    public ILogger WithProperty(string name, object value)
    {
        ExtraProperties.Add(new KeyValuePair<string, object>(name, value));

        return this;
    }
}