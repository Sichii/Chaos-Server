using Chaos.Containers;
using Chaos.Data;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class TimedEventMapperProfile : IMapperProfile<TimedEvent, TimedEventSchema>,
                                       IMapperProfile<TimedEventCollection, TimedEventCollectionSchema>
{
    private readonly ITypeMapper Mapper;

    public TimedEventMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    /// <inheritdoc />
    public TimedEvent Map(TimedEventSchema obj)
    {
        if (!Enum.TryParse<TimedEvent.TimedEventId>(obj.EventId, out var eventId))
            throw new InvalidCastException($"Unable to parse {nameof(TimedEvent.TimedEventId)} from string \"{obj.EventId}\"");

        return new TimedEvent(
            obj.UniqueId,
            eventId,
            obj.Duration,
            obj.Start,
            obj.AutoConsume);
    }

    /// <inheritdoc />
    public TimedEventSchema Map(TimedEvent obj) => new()
    {
        UniqueId = obj.UniqueId,
        EventId = obj.EventId.ToString(),
        Duration = obj.Duration,
        Start = obj.Start,
        AutoConsume = obj.AutoConsume
    };

    /// <inheritdoc />
    public TimedEventCollection Map(TimedEventCollectionSchema obj) => new(Mapper.MapMany<TimedEvent>(obj));

    /// <inheritdoc />
    public TimedEventCollectionSchema Map(TimedEventCollection obj) => new(Mapper.MapMany<TimedEventSchema>(obj));
}