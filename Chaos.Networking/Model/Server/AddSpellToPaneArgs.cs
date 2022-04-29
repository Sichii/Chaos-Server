using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AddSpellToPaneArgs : ISendArgs
{
    public SpellArg Spell { get; set; } = null!;
}