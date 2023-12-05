using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.JsonConverters;

/// <inheritdoc />
public sealed class LocationConverter : JsonConverter<Location>
{
    /// <summary>
    ///     The singleton instance of <see cref="LocationConverter" />
    /// </summary>
    public static JsonConverter<Location> Instance { get; } = new LocationConverter();

    /// <inheritdoc />
    public override Location Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrEmpty(str))
            throw new InvalidOperationException("Expected a string");

        if (Location.TryParse(str, out var location))
            return location;

        throw new InvalidOperationException($"Invalid string format for location. \"{str}\"");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Location value, JsonSerializerOptions options)
        => writer.WriteStringValue(ILocation.ToString(value));
}