using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Shared;
using SeqConfigurator.Utility;

namespace SeqConfigurator.Builders;

public sealed class ChartQueryBuilder
{
    private readonly AsyncComposer<ChartQueryPart> AsyncComposer;
    private readonly SeqConnection SeqConnection;

    private ChartQueryBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        AsyncComposer = AsyncComposer<ChartQueryPart>.Create(new ChartQueryPart());
    }

    public Task<ChartQueryPart> BuildAsync() => AsyncComposer.WaitAsync();

    public static ChartQueryBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public ChartQueryBuilder WithDisplayStyle(
        MeasurementDisplayType type,
        bool lineShowMarkers = true,
        bool lineFillToZeroY = false,
        bool barOverlaySum = false,
        bool suppressLegend = false,
        MeasurementDisplayPalette palette = MeasurementDisplayPalette.Default)
    {
        AsyncComposer.Compose(
            chartQuery =>
            {
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
        AsyncComposer.Compose(
            chartQuery =>
            {
                chartQuery.GroupBy.AddRange(groupByClauses);
            });

        return this;
    }

    public ChartQueryBuilder WithHaving(string havingClause)
    {
        AsyncComposer.Compose(
            chartQuery =>
            {
                chartQuery.Having = havingClause;
            });

        return this;
    }

    public ChartQueryBuilder WithLimit(int limit)
    {
        AsyncComposer.Compose(
            chartQuery =>
            {
                chartQuery.Limit = limit;
            });

        return this;
    }

    public ChartQueryBuilder WithOrderBy(params string[] orderByClauses)
    {
        AsyncComposer.Compose(
            chartQuery =>
            {
                chartQuery.OrderBy.AddRange(orderByClauses);
            });

        return this;
    }

    public ChartQueryBuilder WithSelect(params (string Label, string SelectClause)[] columns)
    {
        AsyncComposer.Compose(
            chartQuery =>
            {
                var measurements = columns.Select(
                    col => new ColumnPart
                    {
                        Label = col.Label,
                        Value = col.SelectClause
                    });

                chartQuery.Measurements.AddRange(measurements);
            });

        return this;
    }

    public ChartQueryBuilder WithSignalExpression(Action<SignalExpressionBuilder> builder)
    {
        AsyncComposer.Compose(
            async chartQuery =>
            {
                var signalExpressionBuilder = SignalExpressionBuilder.Create(SeqConnection);
                builder(signalExpressionBuilder);

                var signalExpression = await signalExpressionBuilder.BuildAsync();

                chartQuery.SignalExpression = signalExpression;
            });

        return this;
    }

    public ChartQueryBuilder WithWhere(string whereClause)
    {
        AsyncComposer.Compose(
            chartQuery =>
            {
                chartQuery.Where = whereClause;
            });

        return this;
    }
}