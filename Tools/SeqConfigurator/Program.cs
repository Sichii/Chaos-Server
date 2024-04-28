using System.Reflection;
using Chaos.Extensions.Common;
using Chaos.NLog.Logging.Definitions;
using NLog;
using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Signals;
using SeqConfigurator.Builders;

using var seq = new SeqConnection("http://localhost:5341", "AdminGuestSeqToken");
await seq.EnsureConnectedAsync(TimeSpan.FromSeconds(30));

var serverTopics = typeof(Topics.Servers).GetProperties(BindingFlags.Static | BindingFlags.Public)
                                         .ToList();

var entityTopics = typeof(Topics.Entities).GetProperties(BindingFlags.Static | BindingFlags.Public)
                                          .ToList();

var actionTopics = typeof(Topics.Actions).GetProperties(BindingFlags.Static | BindingFlags.Public)
                                         .ToList();

var qualifierTopics = typeof(Topics.Qualifiers).GetProperties(BindingFlags.Static | BindingFlags.Public)
                                               .ToList();

Console.WriteLine("Clearing existing signals");

//clear existing signals
foreach (var signal in await seq.Signals.ListAsync(shared: true))
    await seq.Signals.RemoveAsync(signal);

Console.WriteLine("Creating LogLevel signals");

//add loglevel signals
var logLevels = new List<LogLevel>
{
    LogLevel.Trace,
    LogLevel.Debug,
    LogLevel.Info,
    LogLevel.Warn,
    LogLevel.Error,
    LogLevel.Fatal
};

foreach (var logLevel in logLevels)
{
    var level = logLevel.ToString()!.ToUpper();

    await SignalBuilder.Create(seq)
                       .IsShared()
                       .WithTitle(level)
                       .WithDescription(level)
                       .WithFilter($"@Level = '{level}'", filterDescription: $"Filter by LogLevel = {level}")
                       .WithGrouping(SignalGrouping.Explicit, "LogLevels")
                       .SaveAsync();
}

Console.WriteLine("Creating Topics.Servers signals");

foreach (var topic in serverTopics)
    await SignalBuilder.Create(seq)
                       .IsShared()
                       .WithTitle(topic.Name)
                       .WithDescription(topic.Name)
                       .WithFilter($"'{topic.Name}' in Topics", filterDescription: $"Filter by Topics.Servers.{topic.Name}")
                       .WithGrouping(SignalGrouping.Explicit, "Servers")
                       .SaveAsync();

Console.WriteLine("Creating Topics.Entities signals");

foreach (var topic in entityTopics)
    await SignalBuilder.Create(seq)
                       .IsShared()
                       .WithTitle(topic.Name)
                       .WithDescription(topic.Name)
                       .WithFilter($"'{topic.Name}' in Topics", filterDescription: $"Filter by Topics.Entities.{topic.Name}")
                       .WithGrouping(SignalGrouping.Explicit, "Entities")
                       .SaveAsync();

Console.WriteLine("Creating Topics.Actions signals");

foreach (var topic in actionTopics)
    await SignalBuilder.Create(seq)
                       .IsShared()
                       .WithTitle(topic.Name)
                       .WithDescription(topic.Name)
                       .WithFilter($"'{topic.Name}' in Topics", filterDescription: $"Filter by Topics.Actions.{topic.Name}")
                       .WithGrouping(SignalGrouping.Explicit, "Actions")
                       .SaveAsync();

Console.WriteLine("Creating Topics.Qualifiers signals");

foreach (var topic in qualifierTopics)
    await SignalBuilder.Create(seq)
                       .IsShared()
                       .WithTitle(topic.Name)
                       .WithDescription(topic.Name)
                       .WithFilter($"'{topic.Name}' in Topics", filterDescription: $"Filter by Topics.Qualifiers.{topic.Name}")
                       .WithGrouping(SignalGrouping.Explicit, "Qualifiers")
                       .SaveAsync();

Console.WriteLine("Clearing existing dashboards");

var dashboards = await seq.Dashboards.ListAsync(shared: true);

foreach (var dashboard in dashboards)
    if (!dashboard.Title.EqualsI("Overview"))
        await seq.Dashboards.RemoveAsync(dashboard);

Console.WriteLine("Creating Dashboard");

await DashboardBuilder.Create(seq)
                      .IsShared()
                      .WithTitle("Monitor")
                      .WithCharts(
                          BuildMapCountChart,
                          BuildAislingCountChart,
                          BuildDeltasChart,
                          BuildAverageDeltasChart,
                          BuildUpperDeltasChart)
                      .SaveAsync();

Console.WriteLine("Done");
Console.ReadLine();

return;

static void BuildDeltasChart(ChartBuilder chartBuilder)
    => chartBuilder.WithTitle("Average of Median and Upper deltas")
                   .WithSignalExpression("DeltaMonitor")
                   .WithDimensions(12, 2)
                   .WithQuery(
                       queryBuilder => queryBuilder.WithSelect(("UpperDelta", "mean(UpperPercentile)"), ("MedianDelta", "mean(Average)"))
                                                   .WithDisplayStyle(MeasurementDisplayType.Line, false, true));

static void BuildAverageDeltasChart(ChartBuilder chartBuilder)
    => chartBuilder.WithTitle("Average Deltas")
                   .WithSignalExpression("DeltaMonitor")
                   .WithDimensions(12, 2)
                   .WithQuery(
                       queryBuilder => queryBuilder.WithSelect(("AvgDelta", "mean(Average)"))
                                                   .WithGroupBy("Name")
                                                   .WithOrderBy("time desc")
                                                   .WithDisplayStyle(MeasurementDisplayType.Line, false));

static void BuildUpperDeltasChart(ChartBuilder chartBuilder)
    => chartBuilder.WithTitle("Upper Deltas")
                   .WithSignalExpression("DeltaMonitor")
                   .WithDimensions(12, 2)
                   .WithQuery(
                       queryBuilder => queryBuilder.WithSelect(("UpperDelta", "mean(UpperPercentile)"))
                                                   .WithGroupBy("Name")
                                                   .WithOrderBy("time desc")
                                                   .WithDisplayStyle(MeasurementDisplayType.Line, false));

static void BuildAislingCountChart(ChartBuilder chartBuilder)
    => chartBuilder.WithTitle("AislingCount")
                   .WithSignalExpression("Aisling")
                   .WithDimensions(12, 2)
                   .WithQuery(
                       queryBuilder => queryBuilder.WithSelect(("AislingCount", "count(distinct(AislingName))"))
                                                   .WithWhere("Has(AislingName)")
                                                   .WithDisplayStyle(MeasurementDisplayType.Line, false, true));

static void BuildMapCountChart(ChartBuilder chartBuilder)
    => chartBuilder.WithTitle("MapCount")
                   .WithSignalExpression("DeltaMonitor")
                   .WithDimensions(12, 2)
                   .WithQuery(
                       queryBuilder => queryBuilder.WithSelect(("MapCount", "count(distinct(Name))"))
                                                   .WithDisplayStyle(MeasurementDisplayType.Line, false, true));