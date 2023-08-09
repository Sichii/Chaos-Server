using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Signals;

namespace SeqConfigurator.Builders;

public class ChartBuilder
{
    private readonly TaskCompletionSource<ChartPart> Promise;
    private readonly SeqConnection SeqConnection;

    private ChartBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        Promise = new TaskCompletionSource<ChartPart>(TaskCreationOptions.RunContinuationsAsynchronously);

        Promise.SetResult(new ChartPart());
    }

    public Task<ChartPart> BuildAsync() => Promise.Task;

    public static ChartBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public ChartBuilder WithDimensions(int columnCount = 6, int rowCount = 1)
    {
        Promise.Task.ContinueWith(
            async creation => (await creation).DisplayStyle = new ChartDisplayStylePart
            {
                HeightRows = rowCount,
                WidthColumns = columnCount
            });

        return this;
    }

    public ChartBuilder WithQuery(params Action<ChartQueryBuilder>[] builderActions)
    {
        var buildTasks = builderActions.Select(
            action =>
            {
                var builder = ChartQueryBuilder.Create(SeqConnection);
                action(builder);

                return builder.BuildAsync();
            });

        var chartQueriesTask = Task.WhenAll(buildTasks);

        Promise.Task.ContinueWith(
            async creation =>
            {
                var chartPart = await creation;
                var chartQueries = await chartQueriesTask;
                chartPart.Queries.AddRange(chartQueries);
            });

        return this;
    }

    public ChartBuilder WithSignalExpression(string signalId) => WithSignalExpression(
        builder =>
            builder.WithKind(SignalExpressionKind.Signal)
                   .WithSignal(signalId));

    public ChartBuilder WithSignalExpression(Action<SignalExpressionBuilder> builderAction)
    {
        var signalExpressionBuilder = SignalExpressionBuilder.Create(SeqConnection);
        builderAction(signalExpressionBuilder);

        Promise.Task.ContinueWith(async creation => (await creation).SignalExpression = await signalExpressionBuilder.BuildAsync());

        return this;
    }

    public ChartBuilder WithTitle(string title)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Title = title);

        return this;
    }
}