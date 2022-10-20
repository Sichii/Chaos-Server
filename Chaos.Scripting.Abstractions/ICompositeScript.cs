namespace Chaos.Scripting.Abstractions;

public interface ICompositeScript<TScript> : IEnumerable<TScript> where TScript: IScript
{
    void Add(TScript script);

    void Remove(TScript script);

    T? GetComponent<T>() where T: TScript;
}