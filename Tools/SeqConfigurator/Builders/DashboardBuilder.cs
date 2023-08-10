using Seq.Api;
using Seq.Api.Model.Dashboarding;
using Seq.Api.Model.Signals;
using SeqConfigurator.Utility;

namespace SeqConfigurator.Builders;

public sealed class DashboardBuilder
{
    private readonly AsyncFluentComposer<DashboardEntity> AsyncComposer;
    private readonly SeqConnection SeqConnection;

    private DashboardBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        AsyncComposer = AsyncFluentComposer<DashboardEntity>.Create(seqConnection.Dashboards.TemplateAsync());
    }

    public static DashboardBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public DashboardBuilder IsProtected()
    {
        AsyncComposer.Compose(obj => obj.IsProtected = true);

        return this;
    }

    public DashboardBuilder IsShared()
    {
        AsyncComposer.Compose(obj => obj.OwnerId = null);

        return this;
    }

    public async Task SaveAsync()
    {
        var dashboard = await AsyncComposer.BuildAsync();
        await SeqConnection.Dashboards.AddAsync(dashboard);
    }

    public DashboardBuilder WithCharts(params Action<ChartBuilder>[] builderActions)
    {
        AsyncComposer.Compose(
            async dashboard =>
            {
                var buildTasks = builderActions.Select(
                    builderAction =>
                    {
                        var chartBuilder = ChartBuilder.Create(SeqConnection);
                        builderAction(chartBuilder);

                        return chartBuilder.BuildAsync();
                    });

                var charts = await Task.WhenAll(buildTasks);

                dashboard.Charts.AddRange(charts);
            });

        return this;
    }

    public DashboardBuilder WithSignalExpression(string signalTitle) => WithSignalExpression(
        builder => builder.WithKind(SignalExpressionKind.Signal)
                          .WithSignal(signalTitle));

    public DashboardBuilder WithSignalExpression(Action<SignalExpressionBuilder> builderAction)
    {
        AsyncComposer.Compose(
            async dashboard =>
            {
                var signalBuilder = SignalExpressionBuilder.Create(SeqConnection);
                builderAction(signalBuilder);

                dashboard.SignalExpression = await signalBuilder.BuildAsync();
            });

        return this;
    }

    public DashboardBuilder WithTitle(string title)
    {
        AsyncComposer.Compose(
            obj =>
            {
                obj.Title = title;
            });

        return this;
    }
}