#region
using System.Net;
using Chaos.Collections;
using Chaos.Collections.Specialized;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public class TrackersMapperProfile : IMapperProfile<Trackers, TrackersSchema>, IMapperProfile<AislingTrackers, AislingTrackersSchema>
{
    /// <inheritdoc />
    public AislingTrackersSchema Map(AislingTrackers obj)
        => new()
        {
            Counters = obj.Counters,
            Enums = obj.Enums,
            Flags = obj.Flags,
            TimedEvents = obj.TimedEvents,
            LastLogin = obj.LastLogin,
            LastLogout = obj.LastLogout,
            LastIpAddress = obj.LastIpAddress?.ToString(),
            AssociatedIpAddresses = obj.AssociatedIpAddresses
                                       .Select(ip => ip.ToString())
                                       .ToList()
        };

    /// <inheritdoc />
    AislingTrackers IMapperProfile<AislingTrackers, AislingTrackersSchema>.Map(AislingTrackersSchema obj)
        => new()
        {
            Counters = obj.Counters,
            Enums = obj.Enums,
            Flags = obj.Flags,
            TimedEvents = obj.TimedEvents,
            LastLogin = obj.LastLogin,
            LastLogout = obj.LastLogout,
            LastIpAddress = string.IsNullOrEmpty(obj.LastIpAddress) ? null : IPAddress.Parse(obj.LastIpAddress),
            AssociatedIpAddresses = new FixedSet<IPAddress>(10, obj.AssociatedIpAddresses.Select(IPAddress.Parse))
        };

    /// <inheritdoc />
    public TrackersSchema Map(Trackers obj)
        => new()
        {
            Counters = obj.Counters,
            Enums = obj.Enums,
            Flags = obj.Flags,
            TimedEvents = obj.TimedEvents
        };

    /// <inheritdoc />
    public Trackers Map(TrackersSchema obj)
        => new()
        {
            Counters = obj.Counters,
            Enums = obj.Enums,
            Flags = obj.Flags,
            TimedEvents = obj.TimedEvents
        };
}