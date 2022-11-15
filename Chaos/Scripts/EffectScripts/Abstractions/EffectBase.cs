using Chaos.Common.Definitions;
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

    protected Aisling? AislingSubject { get; set; }
    protected TimeSpan Elapsed { get; private set; }
    protected Creature Subject { get; set; } = null!;
    public abstract byte Icon { get; }
    public abstract string Name { get; }
    protected abstract TimeSpan Duration { get; }

    protected EffectColor GetColor()
    {
        var remaining = Remaining;

        return remaining.TotalSeconds switch
        {
            >= 60 => EffectColor.White,
            >= 30 => EffectColor.Red,
            >= 15 => EffectColor.Orange,
            >= 10 => EffectColor.Yellow,
            >= 5  => EffectColor.Green,
            > 0   => EffectColor.Blue,
            _     => EffectColor.None
        };
    }

    public static string GetEffectKey(Type type) => type.Name.Replace("effect", string.Empty, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public virtual void OnApplied(Creature target)
    {
        Subject = target;
        AislingSubject = target as Aisling;
    }

    public virtual void OnDispelled() { }

    /// <inheritdoc />
    public virtual void OnReApplied(Creature target) => OnApplied(target);

    public virtual void OnTerminated() { }

    /// <inheritdoc />
    public virtual bool ShouldApply(Creature source, Creature target) => !target.Effects.Contains(Name);

    public virtual void Update(TimeSpan delta)
    {
        Elapsed += delta;

        var currentColor = GetColor();

        if (Color != currentColor)
        {
            Color = currentColor;
            AislingSubject?.Client.SendEffect(Color, Icon);
        }
    }
}