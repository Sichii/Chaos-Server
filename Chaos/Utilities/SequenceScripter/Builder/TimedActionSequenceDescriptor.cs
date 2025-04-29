#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class TimedActionSequenceDescriptor<T> where T: Creature
{
    public List<TimedActionDescriptor<T>> Sequence = [];
    public int? StartingAtHealthPercent { get; init; }
    public TimeSpan? StartingAtTime { get; init; }
}