using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chaos.Common.Converters;

/// <summary>
///     A converter for <see cref="IEnumerable{T}" />
/// </summary>
/// <typeparam name="T">
///     The type implementing <see cref="IEnumerable{T}" />
/// </typeparam>
/// <typeparam name="TObj">
///     The type of object in the sequence
/// </typeparam>
public sealed class EnumerableConverter<T, TObj> : JsonConverter<T> where T: IEnumerable<TObj>
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = JsonSerializer.Deserialize<ICollection<TObj>>(ref reader, options);

        return (T)Activator.CreateInstance(typeof(T), list!)!;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var list = value.ToList();

        JsonSerializer.Serialize(writer, list, options);
    }
}