using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record LightLevelArgs : ISendArgs
{
    public LightLevel LightLevel { get; set; }
}