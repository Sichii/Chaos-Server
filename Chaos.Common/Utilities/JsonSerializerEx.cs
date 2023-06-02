using System.Text.Json;

namespace Chaos.Common.Utilities;

/// <summary>
///     A static utility for serializing and deserializing objects
/// </summary>
public static class JsonSerializerEx
{
    /// <summary>
    ///     Deserializes a file from the specified path
    /// </summary>
    /// <param name="path">The path to deserialize from</param>
    /// <param name="options">The serialization options to use</param>
    /// <typeparam name="T">The type to deserialize into</typeparam>
    public static T? Deserialize<T>(string path, JsonSerializerOptions options)
    {
        using var stream = File.Open(
            path,
            new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Options = FileOptions.SequentialScan,
                Share = FileShare.ReadWrite
            });

        var ret = JsonSerializer.Deserialize<T>(stream, options);

        return ret;
    }

    /// <summary>
    ///     Asynchronously deserializes a file from the specified path
    /// </summary>
    /// <param name="path">The path to deserialize from</param>
    /// <param name="options">The serialization options to use</param>
    /// <typeparam name="T">The type to deserialize into</typeparam>
    public static async Task<T?> DeserializeAsync<T>(string path, JsonSerializerOptions options)
    {
        await using var stream = File.Open(
            path,
            new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
                Share = FileShare.ReadWrite
            });

        var ret = await JsonSerializer.DeserializeAsync<T>(stream, options);

        return ret;
    }

    /// <summary>
    ///     Serializes an object to the specified path
    /// </summary>
    /// <param name="path">The path to serialize the object to</param>
    /// <param name="value">The object to be serialized</param>
    /// <param name="options">The serialization options to use</param>
    public static void Serialize(string path, object value, JsonSerializerOptions options)
    {
        using var stream = File.Open(
            path,
            new FileStreamOptions
            {
                Access = FileAccess.ReadWrite,
                Mode = FileMode.OpenOrCreate,
                Options = FileOptions.SequentialScan,
                Share = FileShare.ReadWrite
            });

        stream.SetLength(0);

        JsonSerializer.Serialize(stream, value, options);
    }

    /// <summary>
    ///     Serializes an object to the specified path
    /// </summary>
    /// <param name="path">The path to serialize the object to</param>
    /// <param name="value">The object to be serialized</param>
    /// <param name="options">The serialization options to use</param>
    public static async Task SerializeAsync(string path, object value, JsonSerializerOptions options)
    {
        await using var stream = File.Open(
            path,
            new FileStreamOptions
            {
                Access = FileAccess.ReadWrite,
                Mode = FileMode.OpenOrCreate,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
                Share = FileShare.ReadWrite
            });

        stream.SetLength(0);

        await JsonSerializer.SerializeAsync(stream, value, options);
    }
}