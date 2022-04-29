namespace Chaos.Core.Interfaces;

public interface IDeltaUpdatable
{
    ValueTask OnUpdated(TimeSpan delta);
}