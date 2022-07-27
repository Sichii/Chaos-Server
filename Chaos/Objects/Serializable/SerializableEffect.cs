using System.Text.Json.Serialization;
using Chaos.Effects.Interfaces;

namespace Chaos.Objects.Serializable;

public record SerializableEffect
{
    public string Name { get; init; } = null!;
    public int? RemainingSecs { get; init; }
    
    #pragma warning disable CS8618
    //json constructor
    public SerializableEffect() { }
    #pragma warning restore CS8618
    
    public SerializableEffect(IEffect effect)
    {
        Name = effect.Name;

        if (effect.Remaining.HasValue)
            RemainingSecs = Convert.ToInt32(effect.Remaining.Value.TotalSeconds);
    }
}