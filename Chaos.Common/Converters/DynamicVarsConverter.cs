using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.Collections;

namespace Chaos.Common.Converters;

public class DynamicVarsConverter : JsonConverter<DynamicVars>
{
    /// <inheritdoc />
    public override DynamicVars Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dic = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options);
        var scriptVars = new DynamicVars(dic!, options);
        
        return scriptVars;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DynamicVars value, JsonSerializerOptions options)
    {
        var dic = value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        JsonSerializer.Serialize(writer, dic, options);
    }
}