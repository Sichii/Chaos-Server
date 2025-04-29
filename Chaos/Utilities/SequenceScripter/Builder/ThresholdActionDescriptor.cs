#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class ThresholdActionDescriptor<T> where T: Creature
{
    public Action<T> Action { get; }
    public int Threshold { get; }

    public ThresholdActionDescriptor(int threshold, Action<T> action)
    {
        Threshold = threshold;
        Action = action;
    }
}