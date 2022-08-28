using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record EffectArgs : ISendArgs
{
    public EffectColor EffectColor { get; set; }
    public byte EffectIcon { get; set; }
}