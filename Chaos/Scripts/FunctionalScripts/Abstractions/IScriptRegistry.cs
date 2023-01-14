namespace Chaos.Scripts.FunctionalScripts.Abstractions;

public interface IScriptRegistry
{
    static virtual IScriptRegistry Instance => null!;
    T Get<T>(string key);
    void Register(string key, Type type);
}