using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Signals;

namespace SeqConfigurator.Builders;

public class DashboardBuilder
{
    private readonly TaskCompletionSource<DashboardEntity> Promise;
    private readonly SeqConnection SeqConnection;

    private DashboardBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        Promise = new TaskCompletionSource<DashboardEntity>(TaskCreationOptions.RunContinuationsAsynchronously);

        SeqConnection.Dashboards.TemplateAsync()
                     .ContinueWith(async task => Promise.SetResult(await task));
    }

    public static DashboardBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public DashboardBuilder IsProtected()
    {
        Promise.Task.ContinueWith(async creation => (await creation).IsProtected = true);

        return this;
    }

    public DashboardBuilder IsShared()
    {
        Promise.Task.ContinueWith(async creation => (await creation).OwnerId = null);

        return this;
    }

    public async Task SaveAsync() => await SeqConnection.Dashboards.AddAsync(await Promise.Task);

    public DashboardBuilder WithCharts(params Action<ChartBuilder>[] builderActions)
    {
        var buildTasks = builderActions.Select(
            builderAction =>
            {
                var chartBuilder = ChartBuilder.Create(SeqConnection);
                builderAction(chartBuilder);

                return chartBuilder.BuildAsync();
            });

        var chartTasks = Task.WhenAll(buildTasks);

        Promise.Task.ContinueWith(
            async creation =>
            {
                var dashboard = await creation;
                var charts = await chartTasks;
                dashboard.Charts.AddRange(charts);
            });

        return this;
    }

    public DashboardBuilder WithSignalExpression(string signalTitle) => WithSignalExpression(
        builder => builder.WithKind(SignalExpressionKind.Signal)
                          .WithSignal(signalTitle));

    public DashboardBuilder WithSignalExpression(Action<SignalExpressionBuilder> builderAction)
    {
        var signalBuilder = SignalExpressionBuilder.Create(SeqConnection);
        builderAction(signalBuilder);
        var signalTask = signalBuilder.BuildAsync();

        Promise.Task.ContinueWith(async creation => (await creation).SignalExpression = await signalTask);

        return this;
    }

    public DashboardBuilder WithTitle(string title)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Title = title);

        return this;
    }
}