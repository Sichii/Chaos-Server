using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class NotSoBadCurseEffect : NonOverwritableEffectBase
{
    /// <inheritdoc />
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 45,
        AnimationSpeed = 100
    };
    /// <inheritdoc />
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "Big Bad Curse",
        "Bad Curse",
        "Curse",
        "Not So Bad Curse"
    };
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(2);
    /// <inheritdoc />
    public override byte Icon => 5;
    /// <inheritdoc />
    public override string Name => "Not So Bad Curse";
    /// <inheritdoc />
    protected override byte? Sound => 27;
}