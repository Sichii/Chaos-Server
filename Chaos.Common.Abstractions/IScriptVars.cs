namespace Chaos.Common.Abstractions;

public interface IScriptVars
{
    T? Get<T>(string key);
    object? Get(Type type, string key);
}