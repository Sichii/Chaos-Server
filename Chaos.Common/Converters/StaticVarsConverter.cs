#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
#endregion

namespace Chaos.Common.Converters;

/// <summary>
///     A converter for <see cref="StaticVars" />
/// </summary>
public sealed class StaticVarsConverter : JsonConverter<StaticVars>
{
    /// <inheritdoc />
    public override StaticVars Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dynamicVars = JsonSerializer.Deserialize<DynamicVars>(ref reader, options)!;

        return dynamicVars.ToStaticVars();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, StaticVars value, JsonSerializerOptions options)
    {
        var dic = value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        JsonSerializer.Serialize(writer, dic, options);
    }
}