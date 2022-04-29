using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record LightLevelArgs : ISendArgs
{
    public LightLevel LightLevel { get; set; }
}