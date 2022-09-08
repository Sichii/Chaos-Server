using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record AddSpellToPaneArgs : ISendArgs
{
    public SpellInfo Spell { get; set; } = null!;
}