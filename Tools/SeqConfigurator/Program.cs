using System.Reflection;
using Chaos.Extensions.Common;
using Chaos.NLog.Logging.Definitions;
using NLog;
using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Shared;
using Seq.Api.Model.Signals;

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
    var template = await seq.Signals.TemplateAsync();
    template.Title = level;
    template.Description = level;

    template.Filters = new List<DescriptiveFilterPart>
    {
        new()
        {
            Description = $"Filter by LogLevel = {level}",
            Filter = $"@Level = '{level}'",
            FilterNonStrict = $@"@Level = ""{level}"""
        }
    };

    template.Grouping = SignalGrouping.Explicit;
    template.ExplicitGroupName = "LogLevels";

    await seq.Signals.AddAsync(template);
}

Console.WriteLine("Creating Topics.Servers signals");

foreach (var topic in serverTopics)
{
    var template = await seq.Signals.TemplateAsync();
    template.Title = topic.Name;
    template.Description = topic.Name;

    template.Filters = new List<DescriptiveFilterPart>
    {
        new()
        {
            Description = $"Filter by Topics.Servers.{topic.Name}",
            Filter = $"'{topic.Name}' in Topics",
            FilterNonStrict = $@"""{topic.Name}"" in Topics"
        }
    };

    template.Grouping = SignalGrouping.Explicit;
    template.ExplicitGroupName = "Servers";

    await seq.Signals.AddAsync(template);
}

Console.WriteLine("Creating Topics.Entities signals");

foreach (var topic in entityTopics)
{
    var template = await seq.Signals.TemplateAsync();
    template.Title = topic.Name;
    template.Description = topic.Name;

    template.Filters = new List<DescriptiveFilterPart>
    {
        new()
        {
            Description = $"Filter by Topics.Entities.{topic.Name}",
            Filter = $"'{topic.Name}' in Topics",
            FilterNonStrict = $@"""{topic.Name}"" in Topics"
        }
    };

    template.Grouping = SignalGrouping.Explicit;
    template.ExplicitGroupName = "Entities";

    await seq.Signals.AddAsync(template);
}

Console.WriteLine("Creating Topics.Actions signals");

foreach (var topic in actionTopics)
{
    var template = await seq.Signals.TemplateAsync();
    template.Title = topic.Name;
    template.Description = topic.Name;

    template.Filters = new List<DescriptiveFilterPart>
    {
        new()
        {
            Description = $"Filter by Topics.Actions.{topic.Name}",
            Filter = $"'{topic.Name}' in Topics",
            FilterNonStrict = $@"""{topic.Name}"" in Topics"
        }
    };

    template.Grouping = SignalGrouping.Explicit;
    template.ExplicitGroupName = "Actions";

    await seq.Signals.AddAsync(template);
}

Console.WriteLine("Creating Topics.Qualifiers signals");

foreach (var topic in qualifierTopics)
{
    var template = await seq.Signals.TemplateAsync();
    template.Title = topic.Name;
    template.Description = topic.Name;

    template.Filters = new List<DescriptiveFilterPart>
    {
        new()
        {
            Description = $"Filter by Topics.Qualifiers.{topic.Name}",
            Filter = $"'{topic.Name}' in Topics",
            FilterNonStrict = $@"""{topic.Name}"" in Topics"
        }
    };

    template.Grouping = SignalGrouping.Explicit;
    template.ExplicitGroupName = "Qualifiers";

    await seq.Signals.AddAsync(template);
}

Console.WriteLine("Creating Delta Monitor signal");

{
    var template = await seq.Signals.TemplateAsync();
    template.Title = "Delta Monitor";
    template.Description = "Delta Monitor";

    template.Filters = new List<DescriptiveFilterPart>
    {
        new()
        {
            Description = "Filter by Delta Monitor",
            Filter = "@MessageTemplate like 'Delta Monitor%'",
            FilterNonStrict = @"@MessageTemplate like ""Delta Monitor%"""
        }
    };

    template.Grouping = SignalGrouping.Explicit;
    template.ExplicitGroupName = "Delta Monitor";

    await seq.Signals.AddAsync(template);
}

Console.WriteLine("Clearing existing dashboards");

var dashboards = await seq.Dashboards.ListAsync(shared: true);

foreach (var dashboard in dashboards)
    if (!dashboard.Title.EqualsI("Overview"))
        await seq.Dashboards.RemoveAsync(dashboard);

Console.WriteLine("Creating Dashboard");

var signals = await seq.Signals.ListAsync(shared: true);

{
    var dashboard = await seq.Dashboards.TemplateAsync();
    dashboard.OwnerId = null;
    dashboard.Title = "Monitor";
    dashboard.IsProtected = false;
    dashboard.SignalExpression = null;

    dashboard.Charts = new List<ChartPart>
    {
        new()
        {
            Title = "Map Count",
            SignalExpression = new SignalExpressionPart
            {
                Kind = SignalExpressionKind.Signal,
                SignalId = signals.First(signal => signal.Title == "Delta Monitor").Id
            },
            Queries = new List<ChartQueryPart>
            {
                new()
                {
                    Measurements = new List<ColumnPart>
                    {
                        new()
                        {
                            Value = "count(distinct(Name))",
                            Label = "MapCount"
                        }
                    },
                    Where = null,
                    SignalExpression = null,
                    GroupBy = new List<string>(),
                    DisplayStyle = new MeasurementDisplayStylePart
                    {
                        Type = MeasurementDisplayType.Line,
                        LineFillToZeroY = false,
                        LineShowMarkers = true,
                        BarOverlaySum = false,
                        SuppressLegend = false,
                        Palette = MeasurementDisplayPalette.Default
                    }
                }
            }
        },
        new()
        {
            Title = "Aisling Count",
            SignalExpression = new SignalExpressionPart
            {
                Kind = SignalExpressionKind.Signal,
                SignalId = signals.First(signal => signal.Title == "Aisling").Id
            },
            Queries = new List<ChartQueryPart>
            {
                new()
                {
                    Measurements = new List<ColumnPart>
                    {
                        new()
                        {
                            Value = "count(distinct(AislingName))",
                            Label = "AislingCount"
                        }
                    },
                    Where = "Has(AislingName)",
                    GroupBy = new List<string>(),
                    DisplayStyle = new MeasurementDisplayStylePart
                    {
                        Type = MeasurementDisplayType.Line,
                        LineFillToZeroY = false,
                        LineShowMarkers = true,
                        BarOverlaySum = false,
                        SuppressLegend = false,
                        Palette = MeasurementDisplayPalette.Default
                    }
                }
            }
        },
        new()
        {
            Title = "Average Deltas",
            SignalExpression = new SignalExpressionPart
            {
                Kind = SignalExpressionKind.Signal,
                SignalId = signals.First(signal => signal.Title == "Delta Monitor").Id
            },
            Queries = new List<ChartQueryPart>
            {
                new()
                {
                    Measurements = new List<ColumnPart>
                    {
                        new()
                        {
                            Value = "mean(Average)",
                            Label = "Delta"
                        }
                    },
                    GroupBy = new List<string> { "Name" },
                    DisplayStyle = new MeasurementDisplayStylePart
                    {
                        Type = MeasurementDisplayType.Line,
                        LineFillToZeroY = false,
                        LineShowMarkers = true,
                        BarOverlaySum = false,
                        SuppressLegend = false,
                        Palette = MeasurementDisplayPalette.Default
                    }
                }
            }
        },
        new()
        {
            Title = "Mean Deltas",
            SignalExpression = new SignalExpressionPart
            {
                Kind = SignalExpressionKind.Signal,
                SignalId = signals.First(signal => signal.Title == "Delta Monitor").Id
            },
            Queries = new List<ChartQueryPart>
            {
                new()
                {
                    Measurements = new List<ColumnPart>
                    {
                        new()
                        {
                            Value = "mean(Median)",
                            Label = "Delta"
                        }
                    },
                    GroupBy = new List<string> { "Name" },
                    DisplayStyle = new MeasurementDisplayStylePart
                    {
                        Type = MeasurementDisplayType.Line,
                        LineFillToZeroY = false,
                        LineShowMarkers = true,
                        BarOverlaySum = false,
                        SuppressLegend = false,
                        Palette = MeasurementDisplayPalette.Default
                    }
                }
            }
        },
        new()
        {
            Title = "95th% Deltas",
            SignalExpression = new SignalExpressionPart
            {
                Kind = SignalExpressionKind.Signal,
                SignalId = signals.First(signal => signal.Title == "Delta Monitor").Id
            },
            Queries = new List<ChartQueryPart>
            {
                new()
                {
                    Measurements = new List<ColumnPart>
                    {
                        new()
                        {
                            Value = "mean(UpperPercentile)",
                            Label = "Delta"
                        }
                    },
                    GroupBy = new List<string> { "Name" },
                    DisplayStyle = new MeasurementDisplayStylePart
                    {
                        Type = MeasurementDisplayType.Line,
                        LineFillToZeroY = false,
                        LineShowMarkers = true,
                        BarOverlaySum = false,
                        SuppressLegend = false,
                        Palette = MeasurementDisplayPalette.Default
                    }
                }
            }
        },
        new()
        {
            Title = "Max Deltas",
            SignalExpression = new SignalExpressionPart
            {
                Kind = SignalExpressionKind.Signal,
                SignalId = signals.First(signal => signal.Title == "Delta Monitor").Id
            },
            Queries = new List<ChartQueryPart>
            {
                new()
                {
                    Measurements = new List<ColumnPart>
                    {
                        new()
                        {
                            Value = "mean(Max)",
                            Label = "Delta"
                        }
                    },
                    GroupBy = new List<string> { "Name" },
                    DisplayStyle = new MeasurementDisplayStylePart
                    {
                        Type = MeasurementDisplayType.Line,
                        LineFillToZeroY = false,
                        LineShowMarkers = true,
                        BarOverlaySum = false,
                        SuppressLegend = false,
                        Palette = MeasurementDisplayPalette.Default
                    }
                }
            }
        }
    };

    await seq.Dashboards.AddAsync(dashboard);
}