using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.Collections;

namespace Chaos.Common.JsonConverters;

public class DynamicVarsConverter : JsonConverter<DynamicVars>
{
    /// <inheritdoc />
    public override DynamicVars Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dic = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options);
        var scriptVars = new DynamicVars();

        if (dic != null)
            foreach (var kvp in dic)
                if (kvp.Value.ValueKind == JsonValueKind.Number)
                    scriptVars.Set(kvp.Key, kvp.Value.GetDecimal());
                else if (kvp.Value.ValueKind is JsonValueKind.True or JsonValueKind.False)
                    scriptVars.Set(kvp.Key, kvp.Value.GetBoolean());
                else if(kvp.Value.ValueKind == JsonValueKind.String)
                    scriptVars.Set(kvp.Key, kvp.Value.GetString()!);

        return scriptVars;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DynamicVars value, JsonSerializerOptions options) =>
        throw new NotImplementedException();
}