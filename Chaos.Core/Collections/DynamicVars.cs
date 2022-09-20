using System.Text.Json.Serialization;
using Chaos.Core.JsonConverters;

namespace Chaos.Core.Collections;

[JsonConverter(typeof(DynamicVarsConverter))]
public class DynamicVars
{
    private readonly ConcurrentDictionary<string, object> Vars;

    public DynamicVars() => Vars = new ConcurrentDictionary<string, object>();

    public T? Get<T>(string key)
    {
        if (!Vars.TryGetValue(key, out var value))
            return default;

        if (typeof(T).IsAssignableTo(typeof(Enum)))
            return (T)Enum.Parse(typeof(T), value.ToString()!, true);

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public void Set(string key, object obj) => Vars.TryAdd(key, obj);
}