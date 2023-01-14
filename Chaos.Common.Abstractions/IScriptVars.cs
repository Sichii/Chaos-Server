namespace Chaos.Common.Abstractions;

public interface IScriptVars
{
    bool ContainsKey(string key);
    T? Get<T>(string key);
    object? Get(Type type, string key);
    T GetRequired<T>(string key);
}