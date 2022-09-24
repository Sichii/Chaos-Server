namespace Chaos.Time.Abstractions;

public interface IDeltaUpdatable
{
    void Update(TimeSpan delta);
}