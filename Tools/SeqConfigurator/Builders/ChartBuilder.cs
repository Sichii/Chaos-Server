using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Signals;
using SeqConfigurator.Utility;

namespace SeqConfigurator.Builders;

public sealed class ChartBuilder
{
    private readonly AsyncComposer<ChartPart> AsyncComposer;
    private readonly SeqConnection SeqConnection;

    private ChartBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        AsyncComposer = AsyncComposer<ChartPart>.Create(new ChartPart());
    }

    public Task<ChartPart> BuildAsync() => AsyncComposer.WaitAsync();

    public static ChartBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public ChartBuilder WithDimensions(int columnCount = 6, int rowCount = 1)
    {
        AsyncComposer.Compose(
            chartPart =>
            {
                chartPart.DisplayStyle = new ChartDisplayStylePart
                {
                    HeightRows = rowCount,
                    WidthColumns = columnCount
                };
            });

        return this;
    }

    public ChartBuilder WithQuery(params Action<ChartQueryBuilder>[] builderActions)
    {
        AsyncComposer.Compose(
            async chartPart =>
            {
                var buildTasks = builderActions.Select(
                    action =>
                    {
                        var builder = ChartQueryBuilder.Create(SeqConnection);
                        action(builder);

                        return builder.BuildAsync();
                    });

                var chartQueries = await Task.WhenAll(buildTasks);

                chartPart.Queries.AddRange(chartQueries);
            });

        return this;
    }

    public ChartBuilder WithSignalExpression(string signalId)
        => WithSignalExpression(
            builder => builder.WithKind(SignalExpressionKind.Signal)
                              .WithSignal(signalId));

    public ChartBuilder WithSignalExpression(Action<SignalExpressionBuilder> builderAction)
    {
        AsyncComposer.Compose(
            async chartPart =>
            {
                var signalExpressionBuilder = SignalExpressionBuilder.Create(SeqConnection);
                builderAction(signalExpressionBuilder);

                var signalExpression = await signalExpressionBuilder.BuildAsync();

                chartPart.SignalExpression = signalExpression;
            });

        return this;
    }

    public ChartBuilder WithTitle(string title)
    {
        AsyncComposer.Compose(
            chartPart =>
            {
                chartPart.Title = title;
            });

        return this;
    }
}