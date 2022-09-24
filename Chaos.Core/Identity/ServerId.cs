using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.Synchronization;

namespace Chaos.Core.Identity;

/// <summary>
///     A utility for generating unique ids in a thread safe manner. These ids will persist through application restarts.
/// </summary>
public static class ServerId
{
    private static readonly SerializableUniqueId SerializableUniqueId;

    /// <summary>
    ///     Generates the next id in the sequence. This is thread safe.
    /// </summary>
    public static ulong NextId => SerializableUniqueId.NextId();

    static ServerId() => SerializableUniqueId = SerializableUniqueId.Deserialize();
}

[JsonConverter(typeof(SerializableUniqueIdConverter))]
public class SerializableUniqueId
{
    private readonly AutoReleasingMonitor Sync;
    private ulong CurrentId;

    private SerializableUniqueId(ulong currentId)
    {
        Sync = new AutoReleasingMonitor();
        CurrentId = currentId;
    }

    public static SerializableUniqueId Deserialize()
    {
        if (!File.Exists("UniqueId.json"))
        {
            var obj = new SerializableUniqueId(0);
            obj.Serialize();

            return obj;
        }

        using var fileStream = File.OpenRead("UniqueId.json");

        return JsonSerializer.Deserialize<SerializableUniqueId>(fileStream)!;
    }

    public ulong NextId()
    {
        using var @lock = Sync.Enter();

        CurrentId++;
        Serialize();

        return CurrentId;
    }

    private void Serialize()
    {
        using var fileStream = File.Create("UniqueId.json");
        JsonSerializer.Serialize(fileStream, this);
    }

    public class SerializableUniqueIdConverter : JsonConverter<SerializableUniqueId>
    {
        public override SerializableUniqueId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var id = 0UL;

            if (reader.TokenType == JsonTokenType.StartObject)
                reader.Read();

            if (reader.TokenType == JsonTokenType.PropertyName)
                reader.Read();

            if (reader.TokenType == JsonTokenType.Number)
                id = reader.GetUInt64();

            reader.Read();

            return new SerializableUniqueId(id);
        }

        public override void Write(Utf8JsonWriter writer, SerializableUniqueId value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(CurrentId), value.CurrentId);
            writer.WriteEndObject();
        }
    }
}