using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record LightLevelArgs : ISendArgs
{
    public LightLevel LightLevel { get; set; }
}