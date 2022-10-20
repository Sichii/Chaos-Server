using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record EffectArgs : ISendArgs
{
    public EffectColor EffectColor { get; set; }
    public byte EffectIcon { get; set; }
}