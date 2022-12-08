using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chaos.Common.Converters;

public sealed class EnumerableConverter<T, TObj> : JsonConverter<T> where T: IEnumerable<TObj>
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = JsonSerializer.Deserialize<List<TObj>>(ref reader, options);

        return (T)Activator.CreateInstance(typeof(T), list!)!;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var list = value.ToList();

        JsonSerializer.Serialize(writer, list, options);
    }
}