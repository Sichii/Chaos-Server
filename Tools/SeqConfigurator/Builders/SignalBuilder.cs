using Seq.Api;
using Seq.Api.Model.Shared;
using Seq.Api.Model.Signals;

namespace SeqConfigurator.Builders;

public sealed class SignalBuilder
{
    private readonly TaskCompletionSource<SignalEntity> Promise;
    private readonly SeqConnection SeqConnection;

    private SignalBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        Promise = new TaskCompletionSource<SignalEntity>(TaskCreationOptions.RunContinuationsAsynchronously);

        SeqConnection.Signals.TemplateAsync()
                     .ContinueWith(async task => Promise.SetResult(await task));
    }

    public static SignalBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public SignalBuilder IsShared()
    {
        Promise.Task.ContinueWith(async creation => (await creation).OwnerId = null);

        return this;
    }

    public async Task SaveAsync() => await SeqConnection.Signals.AddAsync(await Promise.Task);

    public SignalBuilder WithDescription(string description)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Description = description);

        return this;
    }

    public SignalBuilder WithFilter(string filter, string? filterNonStrict = null, string? filterDescription = null)
    {
        var filterDescriptor = new DescriptiveFilterPart
        {
            Filter = filter,
            FilterNonStrict = filterNonStrict,
            Description = filterDescription
        };

        Promise.Task.ContinueWith(async creation => (await creation).Filters.Add(filterDescriptor));

        return this;
    }

    public SignalBuilder WithGrouping(SignalGrouping grouping, string? groupName = null)
    {
        Promise.Task.ContinueWith(
            async creation =>
            {
                var signal = await creation;
                signal.Grouping = grouping;
                signal.ExplicitGroupName = groupName;
            });

        return this;
    }

    public SignalBuilder WithTitle(string title)
    {
        Promise.Task.ContinueWith(async creation => (await creation).Title = title);

        return this;
    }
}