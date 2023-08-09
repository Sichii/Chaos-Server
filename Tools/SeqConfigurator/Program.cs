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
                          BuildAverageDeltasChart,
                          BuildMedianDeltasChart,
                          Build95ThPctDeltasChart,
                          BuildMaxDeltasChart)
                      .SaveAsync();

Console.WriteLine("Done");

return;

static void BuildAverageDeltasChart(ChartBuilder chartBuilder) =>
    chartBuilder.WithTitle("Average Deltas")
                .WithSignalExpression("DeltaMonitor")
                .WithDimensions(12, 2)
                .WithQuery(
                    queryBuilder =>
                        queryBuilder.WithSelect(("Delta", "mean(Average)"))
                                    .WithGroupBy("Name")
                                    .WithDisplayStyle(MeasurementDisplayType.Line));

static void BuildMedianDeltasChart(ChartBuilder chartBuilder) =>
    chartBuilder.WithTitle("Median Deltas")
                .WithSignalExpression("DeltaMonitor")
                .WithDimensions(12, 2)
                .WithQuery(
                    queryBuilder =>
                        queryBuilder.WithSelect(("Delta", "mean(Median)"))
                                    .WithGroupBy("Name")
                                    .WithDisplayStyle(MeasurementDisplayType.Line));

static void Build95ThPctDeltasChart(ChartBuilder chartBuilder) =>
    chartBuilder.WithTitle("95th% Deltas")
                .WithSignalExpression("DeltaMonitor")
                .WithDimensions(12, 2)
                .WithQuery(
                    queryBuilder =>
                        queryBuilder.WithSelect(("Delta", "mean(UpperPercentile)"))
                                    .WithGroupBy("Name")
                                    .WithDisplayStyle(MeasurementDisplayType.Line));

static void BuildMaxDeltasChart(ChartBuilder chartBuilder) =>
    chartBuilder.WithTitle("Max Deltas")
                .WithSignalExpression("DeltaMonitor")
                .WithDimensions(12, 2)
                .WithQuery(
                    queryBuilder =>
                        queryBuilder.WithSelect(("Delta", "mean(Max)"))
                                    .WithGroupBy("Name")
                                    .WithDisplayStyle(MeasurementDisplayType.Line));

static void BuildAislingCountChart(ChartBuilder chartBuilder) =>
    chartBuilder.WithTitle("AislingCount")
                .WithSignalExpression("Aisling")
                .WithDimensions(12, 2)
                .WithQuery(
                    queryBuilder =>
                        queryBuilder.WithSelect(("MapCount", "count(distinct(Name))"))
                                    .WithWhere("Has(AislingName)")
                                    .WithDisplayStyle(MeasurementDisplayType.Line));

static void BuildMapCountChart(ChartBuilder chartBuilder) =>
    chartBuilder.WithTitle("MapCount")
                .WithSignalExpression("DeltaMonitor")
                .WithDimensions(12, 2)
                .WithQuery(
                    queryBuilder =>
                        queryBuilder.WithSelect(("MapName", "count(distinct(Name))"))
                                    .WithDisplayStyle(MeasurementDisplayType.Line));