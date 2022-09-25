using Chaos.Common.Definitions;

namespace Chaos.Networking.Entities.Server;

public record GroundItemInfo : VisibleObjectInfo
{
    public DisplayColor Color { get; set; }
}