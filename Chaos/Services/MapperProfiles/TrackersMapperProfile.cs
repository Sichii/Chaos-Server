using Chaos.Collections;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class TrackersMapperProfile : IMapperProfile<Trackers, TrackersSchema>
{
    /// <inheritdoc />
    public TrackersSchema Map(Trackers obj) => new()
    {
        Counters = obj.Counters,
        Enums = obj.Enums,
        Flags = obj.Flags,
        TimedEvents = obj.TimedEvents
    };

    /// <inheritdoc />
    public Trackers Map(TrackersSchema obj) => new()
    {
        Counters = obj.Counters,
        Enums = obj.Enums,
        Flags = obj.Flags,
        TimedEvents = obj.TimedEvents
    };
}