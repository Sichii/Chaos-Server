using Chaos.Extensions.Common;
using Seq.Api;
using Seq.Api.Model.Signals;
using SeqConfigurator.Utility;

namespace SeqConfigurator.Builders;

public sealed class SignalExpressionBuilder
{
    private readonly TaskCompletionSource<List<SignalEntity>> AllSignals;
    private readonly AsyncComposer<SignalExpressionPart> AsyncComposer;
    private readonly SeqConnection SeqConnection;

    private SignalExpressionBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        AsyncComposer = AsyncComposer<SignalExpressionPart>.Create(new SignalExpressionPart());
        AllSignals = new TaskCompletionSource<List<SignalEntity>>();

        SeqConnection.Signals.ListAsync(shared: true)
                     .ContinueWith(async task => AllSignals.SetResult(await task), TaskContinuationOptions.ExecuteSynchronously);
    }

    public Task<SignalExpressionPart> BuildAsync() => AsyncComposer.WaitAsync();

    public static SignalExpressionBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public SignalExpressionBuilder WithKind(SignalExpressionKind kind)
    {
        AsyncComposer.Compose(expression => expression.Kind = kind);

        return this;
    }

    public SignalExpressionBuilder WithLeft(SignalExpressionPart left)
    {
        AsyncComposer.Compose(expression => expression.Left = left);

        return this;
    }

    public SignalExpressionBuilder WithLeftSignal(string signalTitle)
    {
        AsyncComposer.Compose(
            async expression =>
            {
                var allSignals = await AllSignals.Task;
                var leftSignal = allSignals.First(s => s.Title.EqualsI(signalTitle));

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
        AsyncComposer.Compose(expression => expression.Right = right);

        return this;
    }

    public SignalExpressionBuilder WithRightSignal(string signalTitle)
    {
        AsyncComposer.Compose(
            async expression =>
            {
                var allSignals = await AllSignals.Task;
                var rightSignal = allSignals.First(s => s.Title.EqualsI(signalTitle));

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
        AsyncComposer.Compose(
            async expression =>
            {
                var allSignals = await AllSignals.Task;
                var signal = allSignals.First(s => s.Title.EqualsI(signalTitle));

                expression.SignalId = signal.Id;
            });

        return this;
    }
}