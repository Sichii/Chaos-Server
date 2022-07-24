using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AddSpellToPaneArgs : ISendArgs
{
    public SpellInfo Spell { get; set; } = null!;
}