using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chaos.Core.Utilities;

public static class ServerId
{
    private static readonly SerializableUniqueId SerializableUniqueId;

    public static ulong NextId => SerializableUniqueId.NextId();

    static ServerId() => SerializableUniqueId = SerializableUniqueId.Deserialize();
}

[JsonConverter(typeof(SerializableUniqueIdConverter))]
public class SerializableUniqueId
{
    private readonly object Sync = new();
    private ulong CurrentId;

    private SerializableUniqueId(ulong currentId) => CurrentId = currentId;

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
        lock (Sync)
        {
            CurrentId++;
            Serialize();

            return CurrentId;
        }
    }

    public void Serialize()
    {
        using var fileStream = File.Create("UniqueId.json");
        JsonSerializer.Serialize(fileStream, this);
    }

    public class SerializableUniqueIdConverter : JsonConverter<SerializableUniqueId>
    {
        public override SerializableUniqueId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var currentId = reader.GetUInt64();

            return new SerializableUniqueId(currentId);
        }

        public override void Write(Utf8JsonWriter writer, SerializableUniqueId value, JsonSerializerOptions options) =>
            writer.WriteNumber(nameof(CurrentId), value.CurrentId);
    }
}