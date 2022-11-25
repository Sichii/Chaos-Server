using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.EffectScripts.Abstractions;

public abstract class EffectBase : IEffect
{
    public EffectColor Color { get; set; }

    public TimeSpan Remaining
    {
        get => Duration - Elapsed;
        set => Elapsed = Duration - value;
    }

    protected Aisling? AislingSubject => Subject as Aisling;
    protected TimeSpan Elapsed { get; private set; }
    public Creature Subject { get; set; } = null!;
    public abstract byte Icon { get; }
    public abstract string Name { get; }
    protected abstract TimeSpan Duration { get; }
    
    public static string GetEffectKey(Type type) => type.Name.Replace("effect", string.Empty, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public virtual void OnApplied() { }

    public virtual void OnDispelled() { }

    /// <inheritdoc />
    public virtual void OnReApplied() => OnApplied();

    public virtual void OnTerminated() { }

    /// <inheritdoc />
    public virtual bool ShouldApply(Creature source, Creature target) => !target.Effects.Contains(Name);

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