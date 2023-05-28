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
    ///     Adds multiple properties to the log event.
    /// </summary>
    /// <param name="logger">The logger to add the property to</param>
    /// <param name="value1">The value of the first property</param>
    /// <param name="name1">
    ///     The name of the first property (this will be auto populated to the variable name you pass in if you
    ///     leave it blank
    /// </param>
    /// <param name="value2">The value of the second property</param>
    /// <param name="name2">
    ///     The name of the second property (this will be auto populated to the variable name you pass in if
    ///     you leave it blank
    /// </param>
    public static ILogger WithProperties(
        this ILogger logger,
        object value1,
        object value2,
        [CallerArgumentExpression(nameof(value1))]
        string name1 = "",
        [CallerArgumentExpression(nameof(value2))]
        string name2 = ""
    ) =>
        logger.WithProperty(value1, name1)
              .WithProperty(value2, name2);

    /// <summary>
    ///     Adds multiple properties to the log event.
    /// </summary>
    /// <param name="logger">The logger to add the property to</param>
    /// <param name="value1">The value of the first property</param>
    /// <param name="name1">
    ///     The name of the first property (this will be auto populated to the variable name you pass in if you
    ///     leave it blank
    /// </param>
    /// <param name="value2">The value of the second property</param>
    /// <param name="name2">
    ///     The name of the second property (this will be auto populated to the variable name you pass in if
    ///     you leave it blank
    /// </param>
    /// <param name="value3">The value of the third property</param>
    /// <param name="name3">
    ///     The name of the third property (this will be auto populated to the variable name you pass in if you
    ///     leave it blank
    /// </param>
    public static ILogger WithProperties(
        this ILogger logger,
        object value1,
        object value2,
        object value3,
        [CallerArgumentExpression(nameof(value1))]
        string name1 = "",
        [CallerArgumentExpression(nameof(value2))]
        string name2 = "",
        [CallerArgumentExpression(nameof(value3))]
        string name3 = ""
    ) =>
        logger.WithProperty(value1, name1)
              .WithProperty(value2, name2)
              .WithProperty(value3, name3);

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
    public int Count => LogValues.Count + ExtraProperties.Count;

    public LogEvent(ILogger logger)
    {
        Logger = logger;
        ExtraProperties = new List<KeyValuePair<string, object>>();
    }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState: notnull => Logger.BeginScope(state);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => LogValues.Concat(ExtraProperties).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => Logger.IsEnabled(logLevel);

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