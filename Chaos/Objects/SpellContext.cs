using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects;

public sealed record SpellContext(Creature Target, Creature Source, string? Prompt = null);