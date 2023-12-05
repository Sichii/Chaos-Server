using System.Collections;
using System.Diagnostics;
using Chaos.Extensions.Common;
using Microsoft.Extensions.Logging;

namespace Chaos.NLog.Logging;

internal sealed class LogEvent : ILogger, IReadOnlyList<KeyValuePair<string, object>>
{
    private readonly IList<KeyValuePair<string, object>> ExtraProperties;
    private readonly ILogger Logger;
    private readonly ICollection<string> Topics;
    private IReadOnlyList<KeyValuePair<string, object>> LogValues = null!;
    private long? StartTimeStamp;

    /// <inheritdoc />
    public int Count => LogValues.Count + ExtraProperties.Count;

    public LogEvent(ILogger logger)
    {
        Logger = logger;
        ExtraProperties = new List<KeyValuePair<string, object>>();
        Topics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        ExtraProperties.Add(new KeyValuePair<string, object>("Topics", Topics));
    }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState: notnull => Logger.BeginScope(state);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        => LogValues.Concat(ExtraProperties)
                    .GetEnumerator();

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
        Func<TState, Exception?, string> formatter)
    {
        if (StartTimeStamp.HasValue)
            ExtraProperties.ReplaceBy(
                kvp => kvp.Key.EqualsI("Metrics"),
                new KeyValuePair<string, object>("Metrics", Stopwatch.GetElapsedTime(StartTimeStamp.Value)));

        LogValues = state as IReadOnlyList<KeyValuePair<string, object>> ?? new List<KeyValuePair<string, object>>();

        Logger.Log(
            logLevel,
            eventId,
            this,
            exception,
            (_, e) => formatter(state, e));
    }

    public ILogger WithMetrics()
    {
        StartTimeStamp = Stopwatch.GetTimestamp();
        ExtraProperties.Add(new KeyValuePair<string, object>("Metrics", null!));

        return this;
    }

    public ILogger WithProperty(string name, object value)
    {
        var type = value.GetType();

        if (name.EqualsI("this"))
            name = type.Name;

        if (!char.IsUpper(name[0]))
            name = name.FirstUpper();

        ExtraProperties.Add(new KeyValuePair<string, object>(name, value));

        return WithTopic(type.Name);
    }

    public ILogger WithTopic(string topic)
    {
        if (!char.IsUpper(topic[0]))
            topic = topic.FirstUpper();

        Topics.Add(topic);

        return this;
    }
}