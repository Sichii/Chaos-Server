using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects;

public record ActivationContext(Creature Target, Creature Source, string? Prompt = null);