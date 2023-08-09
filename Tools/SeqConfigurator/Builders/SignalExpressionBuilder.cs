using Chaos.Extensions.Common;
using Seq.Api;
using Seq.Api.Model.Signals;

namespace SeqConfigurator.Builders;

public class SignalExpressionBuilder
{
    private readonly TaskCompletionSource<List<SignalEntity>> AllSignals;
    private readonly TaskCompletionSource<SignalExpressionPart> Promise;
    private readonly SeqConnection SeqConnection;

    private SignalExpressionBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        Promise = new TaskCompletionSource<SignalExpressionPart>(TaskCreationOptions.RunContinuationsAsynchronously);
        AllSignals = new TaskCompletionSource<List<SignalEntity>>();

        Promise.TrySetResult(new SignalExpressionPart());

        SeqConnection.Signals.ListAsync(shared: true)
                     .ContinueWith(async task => AllSignals.SetResult(await task));
    }

    public Task<SignalExpressionPart> BuildAsync() => Promise.Task;

    public static SignalExpressionBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public SignalExpressionBuilder WithKind(SignalExpressionKind kind)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Kind = kind);

        return this;
    }

    public SignalExpressionBuilder WithLeft(SignalExpressionPart left)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Left = left);

        return this;
    }

    public SignalExpressionBuilder WithLeftSignal(string signalTitle)
    {
        Promise.Task.ContinueWith(
            async creation =>
            {
                var allSignals = await AllSignals.Task;
                var leftSignal = allSignals.First(s => s.Title.EqualsI(signalTitle));
                var expression = await creation;

                expression.Left = new SignalExpressionPart
                {
                    Kind = SignalExpressionKind.Signal,
                    SignalId = leftSignal.Id
                };
            });

        return this;
    }

    public SignalExpressionBuilder WithRight(SignalExpressionPart right)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Right = right);

        return this;
    }

    public SignalExpressionBuilder WithRightSignal(string signalTitle)
    {
        Promise.Task.ContinueWith(
            async creation =>
            {
                var allSignals = await AllSignals.Task;
                var rightSignal = allSignals.First(s => s.Title.EqualsI(signalTitle));
                var expression = await creation;

                expression.Right = new SignalExpressionPart
                {
                    Kind = SignalExpressionKind.Signal,
                    SignalId = rightSignal.Id
                };
            });

        return this;
    }

    public SignalExpressionBuilder WithSignal(string signalTitle)
    {
        Promise.Task.ContinueWith(
            async creation =>
            {
                var allSignals = await AllSignals.Task;
                var signal = allSignals.First(s => s.Title.EqualsI(signalTitle));
                var expression = await creation;

                expression.SignalId = signal.Id;
            });

        return this;
    }
}