using Chaos.Common.Definitions;

namespace Chaos.Networking.Entities.Server;

public sealed record GroundItemInfo : VisibleObjectInfo
{
    public DisplayColor Color { get; set; }
}