using Chaos.Core.Definitions;

namespace Chaos.Networking.Model.Server;

public record GroundItemArg : VisibleArg
{
    public DisplayColor Color { get; set; }
}