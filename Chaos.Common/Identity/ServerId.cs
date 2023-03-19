using System.Text.Json;
using Chaos.Common.Synchronization;

namespace Chaos.Common.Identity;

/// <summary>
///     A static utility for generating unique ids in a threadsafe manner
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

/// <summary>
///     A class for generating unique ids in a thread safe manner. These ids will persist through application restarts.
/// </summary>
public sealed class SerializableUniqueId
{
    private readonly FifoSemaphoreSlim Sync;
    private ulong CurrentId;

    private SerializableUniqueId(ulong currentId)
    {
        CurrentId = currentId;
        Sync = new FifoSemaphoreSlim(1, 1);
    }

    /// <summary>
    ///     Deserializes the current id from disk. If the file does not exist, it will be created.
    /// </summary>
    public static SerializableUniqueId Deserialize()
    {
        if (!File.Exists("UniqueId.json"))
        {
            var obj = new SerializableUniqueId(1);
            obj.Serialize(obj.CurrentId);

            return obj;
        }

        using var fileStream = File.OpenRead("UniqueId.json");

        if (fileStream.Length == 0)
        {
            var obj = new SerializableUniqueId(1);
            obj.Serialize(obj.CurrentId);

            return obj;
        }

        var num = JsonSerializer.Deserialize<ulong>(fileStream)!;

        return new SerializableUniqueId(num);
    }

    /// <summary>
    ///     Generates the next id in the sequence. This is thread safe.
    /// </summary>
    /// <returns></returns>
    public ulong NextId()
    {
        var num = Interlocked.Increment(ref CurrentId);
        Serialize(num);

        return num;
    }

    private async void Serialize(ulong num)
    {
        await Sync.WaitAsync();
        await Task.Yield();

        try
        {
            var json = JsonSerializer.Serialize(num);

            // ReSharper disable once MethodHasAsyncOverload
            File.WriteAllText("UniqueId.json", json);
        } catch
        {
            //ignored
        } finally
        {
            Sync.Release();
        }
    }
}