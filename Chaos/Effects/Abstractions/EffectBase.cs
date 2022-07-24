using Chaos.Effects.Interfaces;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Effects.Abstractions;

public abstract class EffectBase : IEffect
{
    public byte Icon { get; init; }

    public TimeSpan? Remaining
    {
        get => Duration - Elapsed;
        set
        {
            //not expected to be called in parallel to Update(delta)
            if (Duration.HasValue && value.HasValue)
                Elapsed = Duration.Value - value.Value;
        }
    }

    protected EffectColor Color { get; set; } = EffectColor.None;
    protected TimeSpan? Duration { get; init; } = TimeSpan.Zero;
    protected TimeSpan Elapsed { get; set; }
    protected Aisling? SourceUser { get; set; }
    protected Aisling? TargetUser { get; set; }
    protected uint UpdateRateMs { get; init; }

    public virtual string CommonIdentifier => Name;
    public virtual string Name { get; }
    protected Creature Source { get; }
    protected Creature Target { get; }

    protected EffectBase(Creature source, Creature target)
    {
        Source = source;
        Target = target;
        SourceUser = source as Aisling;
        TargetUser = target as Aisling;
        Name = GetEffectKey(GetType());
    }

    public virtual bool Equals(IEffect? other) => other is not null && (other.Icon == Icon);

    protected EffectColor GetColor()
    {
        var remaining = Remaining;

        if (remaining == null)
            return EffectColor.White;

        return remaining.Value.Seconds switch
        {
            >= 60 => EffectColor.White,
            >= 30 => EffectColor.Red,
            >= 15 => EffectColor.Orange,
            >= 10 => EffectColor.Yellow,
            >= 5  => EffectColor.Green,
            >= 0  => EffectColor.Blue,
            _     => EffectColor.None
        };
    }

    public static string GetEffectKey(Type type) => type.Name.Replace("effect", string.Empty, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => Icon.GetHashCode();

    public virtual void OnApplied() => SendColor();
    public virtual void OnDispelled() => RemoveEffect();
    public virtual void OnFailedToApply(string reason) => SourceUser?.Client.SendServerMessage(ServerMessageType.ActiveMessage, reason);
    public virtual void OnTerminated() => RemoveEffect();
    public virtual void OnUpdated() => SendColor();

    protected void RemoveEffect() => SourceUser?.Client.SendEffect(EffectColor.None, Icon);

    protected void SendColor()
    {
        if (SourceUser is not null && ShouldSendColor())
            SourceUser.Client.SendEffect(Color, Icon);
    }

    protected bool ShouldSendColor()
    {
        var currentColor = GetColor();

        if (Color != currentColor)
        {
            Color = currentColor;

            return true;
        }

        return false;
    }

    public virtual void Update(TimeSpan delta) => Elapsed += delta;
}