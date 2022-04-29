using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.Abstractions;

public interface IScript
{
    public string ScriptKey { get; }
}

public abstract class ScriptBase<T> : IScript where T: IScripted
{
    public string ScriptKey { get; set; } = null!;
    protected virtual T Obj { get; }

    protected ScriptBase(T obj) => Obj = obj;
}