using Chaos.Common.Definitions;

namespace Chaos.Entities.Networking.Server;

public record GroundItemInfo : VisibleObjectInfo
{
    public DisplayColor Color { get; set; }
}