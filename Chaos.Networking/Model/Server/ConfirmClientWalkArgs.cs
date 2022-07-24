using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ConfirmClientWalkArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public IPoint OldPoint { get; set; } = null!;
}