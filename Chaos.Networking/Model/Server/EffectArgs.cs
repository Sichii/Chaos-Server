using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record EffectArgs : ISendArgs
{
    public EffectColor EffectColor { get; set; }
    public byte EffectIcon { get; set; }
}