namespace Chaos.Time.Interfaces;

public interface IDeltaUpdatable
{
    void Update(TimeSpan delta);
}