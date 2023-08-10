using Seq.Api;
using Seq.Api.Model.Shared;
using Seq.Api.Model.Signals;
using SeqConfigurator.Utility;

namespace SeqConfigurator.Builders;

public sealed class SignalBuilder
{
    private readonly AsyncFluentComposer<SignalEntity> AsyncComposer;
    private readonly SeqConnection SeqConnection;

    private SignalBuilder(SeqConnection seqConnection)
    {
        SeqConnection = seqConnection;
        AsyncComposer = AsyncFluentComposer<SignalEntity>.Create(seqConnection.Signals.TemplateAsync());
    }

    public static SignalBuilder Create(SeqConnection seqConnection) => new(seqConnection);

    public SignalBuilder IsShared()
    {
        AsyncComposer.Compose(obj => obj.OwnerId = null);

        return this;
    }

    public async Task SaveAsync()
    {
        var signal = await AsyncComposer.BuildAsync();
        await SeqConnection.Signals.AddAsync(signal);
    }

    public SignalBuilder WithDescription(string description)
    {
        AsyncComposer.Compose(obj => obj.Description = description);

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

        AsyncComposer.Compose(obj => obj.Filters.Add(filterDescriptor));

        return this;
    }

    public SignalBuilder WithGrouping(SignalGrouping grouping, string? groupName = null)
    {
        AsyncComposer.Compose(
            obj =>
            {
                obj.Grouping = grouping;
                obj.ExplicitGroupName = groupName;
            });

        return this;
    }

    public SignalBuilder WithTitle(string title)
    {
        AsyncComposer.Compose(obj => obj.Title = title);

        return this;
    }
}