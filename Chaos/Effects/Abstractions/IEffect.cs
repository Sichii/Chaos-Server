using Chaos.Time.Abstractions;

namespace Chaos.Effects.Abstractions;

public interface IEffect : IDeltaUpdatable, IEquatable<IEffect>
{
    TimeSpan? Remaining { get; set; }
    string CommonIdentifier { get; }
    byte Icon { get; }
    string Name { get; }
    void OnApplied();
    void OnDispelled();
    void OnFailedToApply(string reason);
    void OnTerminated();
    void OnUpdated();
}