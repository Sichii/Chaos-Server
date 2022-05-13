namespace Chaos.Core.Interfaces;

public interface IDeltaUpdatable
{
    void Update(TimeSpan delta);
}