using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public abstract class EffectBase : IEffect
{
    public EffectColor Color { get; set; }
    protected abstract TimeSpan Duration { get; set; }
    protected TimeSpan Elapsed { get; private set; }

    public TimeSpan Remaining
    {
        get => Duration - Elapsed;
        set => Elapsed = Duration - value;
    }

    public Creature Subject { get; set; } = null!;
    public abstract byte Icon { get; }
    public abstract string Name { get; }

    /// <inheritdoc />
    public string ScriptKey { get; }

    public Aisling? AislingSubject => Subject as Aisling;

    protected EffectBase() => ScriptKey = GetEffectKey(GetType());

    /// <inheritdoc />
    public virtual void OnApplied() { }

    public virtual void OnDispelled() => OnTerminated();

    /// <inheritdoc />
    public virtual void OnReApplied() => OnApplied();

    public virtual void OnTerminated() { }

    /// <inheritdoc />
    public void SetDuration(TimeSpan duration) => Duration = duration;

    /// <inheritdoc />
    public virtual bool ShouldApply(Creature source, Creature target)
    {
        var conflictingEffect = target.Effects.FirstOrDefault(e => e.Name.EqualsI(Name) || (e.Icon == Icon));

        if (conflictingEffect is null) return true;
        if (source is Aisling aisling)
            aisling.SendActiveMessage($"Target is already affected by {conflictingEffect.Name}.");

        return false;

    }

    public virtual void Update(TimeSpan delta)
    {
        Elapsed += delta;

        var currentColor = this.GetColor();

        if (Color == currentColor) return;
        Color = currentColor;
        AislingSubject?.Client.SendEffect(Color, Icon);
    }

    public static string GetEffectKey(Type type) => type.Name.ReplaceI("effect", string.Empty);
}