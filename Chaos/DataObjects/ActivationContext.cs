using Chaos.PanelObjects.Abstractions;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.DataObjects;

public record ActivationContext(PanelObjectBase Invoker, Creature Target, Creature Source, string? Prompt = null);