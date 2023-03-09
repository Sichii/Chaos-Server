using Chaos.Collections.Common;
using Chaos.Collections.Time;

namespace Chaos.Schemas.Aisling;

public class TrackersSchema
{
    public CounterCollection Counters { get; set; }
    public EnumCollection Enums { get; set; }
    public FlagCollection Flags { get; set; }
    public TimedEventCollection TimedEvents { get; set; }
}