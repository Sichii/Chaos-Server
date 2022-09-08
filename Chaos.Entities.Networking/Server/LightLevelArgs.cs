using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record LightLevelArgs : ISendArgs
{
    public LightLevel LightLevel { get; set; }
}