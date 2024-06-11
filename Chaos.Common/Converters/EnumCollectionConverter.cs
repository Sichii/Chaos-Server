using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;

namespace Chaos.Common.Converters;

/// <summary>
///     A converter for <see cref="EnumCollection" />
/// </summary>
public sealed class EnumCollectionConverter : JsonConverter<EnumCollection>
{
    /// <inheritdoc />
    public override EnumCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var serializedDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);

        var flagCollection = new EnumCollection();

        var possibleTypes = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .Where(a => a is { IsDynamic: false, ReflectionOnly: false })
                                     .SelectMany(
                                         a =>
                                         {
                                             try
                                             {
                                                 return a.GetTypes();
                                             } catch
                                             {
                                                 return [];
                                             }
                                         })
                                     .Where(asmType => asmType.IsEnum && asmType is { IsInterface: false, IsAbstract: false })
                                     .ToList();

        foreach (var kvp in serializedDictionary!)
        {
            var flagType = possibleTypes.Single(type => type.Name.Equals(kvp.Key));
            var flagValue = (Enum)Enum.Parse(flagType, kvp.Value);
            flagCollection.Set(flagType, flagValue);
        }

        return flagCollection;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EnumCollection value, JsonSerializerOptions options)
    {
        var serializedDictionary = new Dictionary<string, string>();

        foreach (var kvp in value)
        {
            var flagName = kvp.Key.Name;

            var flagValue = Enum.ToObject(kvp.Key, kvp.Value)
                                .ToString();

            serializedDictionary.Add(flagName, flagValue!);
        }

        JsonSerializer.Serialize(writer, serializedDictionary, options);
    }
}