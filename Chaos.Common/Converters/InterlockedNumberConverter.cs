using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.Synchronization;

namespace Chaos.Common.Converters;

public class InterlockedNumberConverter : JsonConverter<object>
{
    /// <inheritdoc />
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var numberType = typeToConvert.GetGenericArguments().First();
        var number = JsonSerializer.Deserialize(ref reader, numberType, options);

        return Activator.CreateInstance(typeToConvert, number);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var numberType = value.GetType().GetGenericArguments().First();
        var interlockedNumberType = value.GetType();
        var number = interlockedNumberType.GetMethod(nameof(InterlockedNumber<int>.Get))!.Invoke(value, null)!;

        JsonSerializer.Serialize(
            writer,
            number,
            numberType,
            options);
    }
}