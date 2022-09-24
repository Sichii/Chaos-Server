using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.JsonConverters;

namespace Chaos.Common.Collections;

[JsonConverter(typeof(DynamicVarsConverter))]
public class DynamicVars
{
    private static readonly JsonSerializerOptions JsonOptions;
    private readonly ConcurrentDictionary<string, object> Vars;

    static DynamicVars() =>
        JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            IgnoreReadOnlyProperties = true,
            IgnoreReadOnlyFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

    public DynamicVars() => Vars = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    public T? Get<T>(string key) => (T?)Get(typeof(T), key);

    public object? Get(Type type, string key)
    {
        if (!Vars.TryGetValue(key, out var value))
            return default;
        
        //if the type is a nullable primitive
        var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

        //grab it's underlying type
        if (nullableUnderlyingType != null)
            type = nullableUnderlyingType;
        
        //if it's an enum, parse from string
        if (type.IsEnum)
            return Enum.Parse(type, value.ToString()!, true);

        //if it's an object, deserialize it
        if (value is JsonElement jElement)
            return jElement.Deserialize(type, JsonOptions);

        //if it's a primitive, convert it
        return Convert.ChangeType(value, type);
    }

    public void Set(string key, object obj) => Vars.TryAdd(key, obj);
}