namespace Chaos.Data;

public class ScriptVars
{
    private readonly ConcurrentDictionary<string, object> Vars;

    public ScriptVars() => Vars = new ConcurrentDictionary<string, object>();

    public T? Get<T>(string key)
    {
        if (!Vars.TryGetValue(key, out var value) || value is not T t)
            return default;
        
        return t;
    }

    public void Set(string key, object obj) => Vars.TryAdd(key, obj);
}