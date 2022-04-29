using System;
using System.Threading.Tasks;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.DataObjects;
using Chaos.Effects.Interfaces;

namespace Chaos.Effects.Abstractions;

public abstract record EffectBase : IEffect
{
    public Animation? Animation { get; init; }
    public EffectColor Color { get; set; } = EffectColor.None;
    public ushort DisplayImage { get; init; }
    public TimeSpan Duration { get; init; } = TimeSpan.Zero;
    public DateTime StartTime { get; init; } = DateTime.UtcNow;
    public uint UpdateRateMs { get; init; }
    public TimeSpan Elapsed => DateTime.UtcNow - StartTime;
    public abstract string Name { get; }

    public virtual bool Equals(EffectBase? other) => other is not null && (other.DisplayImage == DisplayImage);

    public EffectColor GetColor() =>
        RemainingDurationMS() switch
        {
            >= 60000 => EffectColor.White,
            >= 45000 => EffectColor.Red,
            >= 30000 => EffectColor.Orange,
            >= 15000 => EffectColor.Yellow,
            >= 10000 => EffectColor.Green,
            >= 5000  => EffectColor.Blue,
            _        => EffectColor.None
        };

    public override int GetHashCode() => DisplayImage.GetHashCode();
    public abstract ValueTask OnUpdated(TimeSpan delta);
    public int RemainingDurationMS() => Convert.ToInt32((Duration - Elapsed).TotalMilliseconds);

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

    public abstract ValueTask<bool> TryApplyAsync(ActivationContext activationContext);
}