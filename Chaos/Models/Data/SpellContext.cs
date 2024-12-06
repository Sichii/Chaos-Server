#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Models.Data;

public sealed record SpellContext : ActivationContext
{
    /// <summary>
    ///     The response to the prompt entered by the activator
    /// </summary>
    public string? PromptResponse { get; init; }

    /// <inheritdoc />
    public SpellContext(Creature source, Creature target, string? promptResponse = null)
        : base(source, target)
        => PromptResponse = promptResponse;
}