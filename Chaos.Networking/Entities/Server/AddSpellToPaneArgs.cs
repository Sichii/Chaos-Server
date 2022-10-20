using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record AddSpellToPaneArgs : ISendArgs
{
    public SpellInfo Spell { get; set; } = null!;
}