using Chaos.Models.World.Abstractions;

namespace Chaos.Models.Data;

public sealed class SpellContext : ActivationContext
{
    public string? Prompt { get; }

    /// <inheritdoc />
    public SpellContext(Creature source, Creature target, string? prompt = null)
        : base(source, target) => Prompt = prompt;
}