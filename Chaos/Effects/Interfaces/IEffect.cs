using Chaos.Core.Interfaces;
using Chaos.Time.Interfaces;

namespace Chaos.Effects.Interfaces;

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