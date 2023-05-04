using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public abstract class EffectBase : IEffect
{
    public EffectColor Color { get; set; }

    public TimeSpan Remaining
    {
        get => Duration - Elapsed;
        set => Elapsed = Duration - value;
    }

    public Creature Subject { get; set; } = null!;
    protected TimeSpan Elapsed { get; private set; }
    public abstract byte Icon { get; }
    public abstract string Name { get; }

    /// <inheritdoc />
    public string ScriptKey { get; }
    protected Aisling? AislingSubject => Subject as Aisling;
    protected abstract TimeSpan Duration { get; }

    protected EffectBase() => ScriptKey = GetEffectKey(GetType());

    public static string GetEffectKey(Type type) => type.Name.ReplaceI("effect", string.Empty);

    /// <inheritdoc />
    public virtual void OnApplied() { }

    public virtual void OnDispelled() => OnTerminated();

    /// <inheritdoc />
    public virtual void OnReApplied() => OnApplied();

    public virtual void OnTerminated() { }

    /// <inheritdoc />
    public virtual bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains(Name))
        {
            AislingSubject?.SendOrangeBarMessage($"You are already affected by {Name}.");

            return false;
        }

        return true;
    }

    public virtual void Update(TimeSpan delta)
    {
        Elapsed += delta;

        var currentColor = this.GetColor();

        if (Color != currentColor)
        {
            Color = currentColor;
            AislingSubject?.Client.SendEffect(Color, Icon);
        }
    }
}