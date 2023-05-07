using Chaos.Models.World.Abstractions;

namespace Chaos.Models.Data;

public sealed record SpellContext : ActivationContext
{
    public string? Prompt { get; init; }

    /// <inheritdoc />
    public SpellContext(Creature source, Creature target, string? prompt = null)
        : base(source, target) => Prompt = prompt;
}