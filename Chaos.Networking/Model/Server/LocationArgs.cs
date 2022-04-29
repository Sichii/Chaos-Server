using Chaos.Core.Geometry;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record LocationArgs : ISendArgs
{
    public Point Point { get; set; }
}