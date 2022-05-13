using System;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Effects.Interfaces;
using Chaos.Objects;

namespace Chaos.Effects.Abstractions;

public abstract class EffectBase : IEffect
{
    public Animation? Animation { get; init; }
    public EffectColor Color { get; set; } = EffectColor.None;
    public ushort DisplayImage { get; init; }
    public TimeSpan Duration { get; init; } = TimeSpan.Zero;
    public TimeSpan Elapsed { get; set; }
    public uint UpdateRateMs { get; init; }
    public abstract string Name { get; }
    public TimeSpan Remaining => Duration - Elapsed;

    public abstract bool Apply(ActivationContext activationContext);

    public virtual bool Equals(EffectBase? other) => other is not null && (other.DisplayImage == DisplayImage);

    public EffectColor GetColor() =>
        Remaining.Seconds switch
        {
            >= 60 => EffectColor.White,
            >= 45 => EffectColor.Red,
            >= 30 => EffectColor.Orange,
            >= 15 => EffectColor.Yellow,
            >= 10 => EffectColor.Green,
            >= 5  => EffectColor.Blue,
            _     => EffectColor.None
        };

    public override int GetHashCode() => DisplayImage.GetHashCode();

    public bool ShouldSendColor()
    {
        var currentColor = GetColor();

        if (Color != currentColor)
        {
            Color = currentColor;

            return true;
        }

        return false;
    }

    public abstract void Update(TimeSpan delta);
}