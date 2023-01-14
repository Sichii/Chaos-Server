using Chaos.Objects.World.Abstractions;

namespace Chaos.Data;

public sealed class SpellContext : ActivationContext
{
    public string? Prompt { get; }

    /// <inheritdoc />
    public SpellContext(Creature source, Creature target, string? prompt = null)
        : base(source, target) => Prompt = prompt;
}