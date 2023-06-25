using Chaos.Collections.Common;
using Chaos.Common.Abstractions;
using Chaos.Models.Templates.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Models.Templates;

public sealed record MerchantTemplate : ITemplate, IScripted
{
    public required Dictionary<string, int> DefaultStock { get; init; }
    public required CounterCollection ItemsForSale { get; init; }

    public required ICollection<string> ItemsToBuy { get; init; }
    public required string Name { get; init; }

    public required int RestockIntervalHrs { get; init; }

    public required int RestockPct { get; init; }
    /// <inheritdoc />
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required IDictionary<string, IScriptVars> ScriptVars { get; init; } =
        new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);

    public required ICollection<string> SkillsToTeach { get; init; }

    public required ICollection<string> SpellsToTeach { get; init; }
    public required ushort Sprite { get; init; }
    /// <inheritdoc />
    public required string TemplateKey { get; init; }

    public required int WanderIntervalMs { get; init; }
}