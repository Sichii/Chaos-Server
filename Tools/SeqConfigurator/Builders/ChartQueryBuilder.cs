using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Shared;

namespace SeqConfigurator.Builders;

public class ChartQueryBuilder
{
    private readonly TaskCompletionSource<ChartQueryPart> Promise;
    private readonly SeqConnection SeqConnection;

    private ChartQueryBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        Promise = new TaskCompletionSource<ChartQueryPart>(TaskCreationOptions.RunContinuationsAsynchronously);

        Promise.SetResult(new ChartQueryPart());
    }

    public Task<ChartQueryPart> BuildAsync() => Promise.Task;

    public static ChartQueryBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public ChartQueryBuilder WithDisplayStyle(
        MeasurementDisplayType type,
        bool lineShowMarkers = true,
        bool lineFillToZeroY = false,
        bool barOverlaySum = false,
        bool suppressLegend = false,
        MeasurementDisplayPalette palette = MeasurementDisplayPalette.Default
    )
    {
        Promise.Task.ContinueWith(
            async creation =>
            {
                var chartQuery = await creation;

                chartQuery.DisplayStyle = new MeasurementDisplayStylePart
                {
                    Type = type,
                    LineShowMarkers = lineShowMarkers,
                    LineFillToZeroY = lineFillToZeroY,
                    BarOverlaySum = barOverlaySum,
                    SuppressLegend = suppressLegend,
                    Palette = palette
                };
            });

        return this;
    }

    public ChartQueryBuilder WithGroupBy(params string[] groupByClauses)
    {
        Promise.Task.ContinueWith(async creation => (await creation).GroupBy.AddRange(groupByClauses));

        return this;
    }

    public ChartQueryBuilder WithLimit(int limit)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Limit = limit);

        return this;
    }

    public ChartQueryBuilder WithOrderBy(params string[] orderByClauses)
    {
        Promise.Task.ContinueWith(async creation => (await creation).OrderBy.AddRange(orderByClauses));

        return this;
    }

    public ChartQueryBuilder WithSelect(params (string Label, string SelectClause)[] columns)
    {
        Promise.Task.ContinueWith(
            async creation =>
            {
                var measurements = columns.Select(
                    col => new ColumnPart
                    {
                        Label = col.Label,
                        Value = col.SelectClause
                    });

                (await creation).Measurements.AddRange(measurements);
            });

        return this;
    }

    public ChartQueryBuilder WithSignalExpression(Action<SignalExpressionBuilder> builder)
    {
        var signalExpressionBuilder = SignalExpressionBuilder.Create(SeqConnection);
        builder(signalExpressionBuilder);

        Promise.Task.ContinueWith(async creation => (await creation).SignalExpression = await signalExpressionBuilder.BuildAsync());

        return this;
    }

    public ChartQueryBuilder WithWhere(string whereClause)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Where = whereClause);

        return this;
    }
}