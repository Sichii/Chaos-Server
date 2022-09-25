using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record AddSpellToPaneArgs : ISendArgs
{
    public SpellInfo Spell { get; set; } = null!;
}