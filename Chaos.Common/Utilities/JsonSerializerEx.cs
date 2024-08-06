using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Chaos.IO.Exceptions;
using Chaos.IO.FileSystem;
using Chaos.IO.Json;

namespace Chaos.Common.Utilities;

/// <summary>
///     A static utility for serializing and deserializing objects
/// </summary>
[ExcludeFromCodeCoverage(Justification = "No logic, just a wrapper utility")]
public static class JsonSerializerEx
{
    /// <summary>
    ///     Deserializes a file from the specified path
    /// </summary>
    /// <param name="path">
    ///     The path to deserialize from
    /// </param>
    /// <param name="options">
    ///     The serialization options to use
    /// </param>
    /// <typeparam name="T">
    ///     The type to deserialize into
    /// </typeparam>
    /// <exception cref="IOException">
    ///     Failed to deserialize object from file, temp file, or backup file. See inner exception for details.
    /// </exception>
    public static T? Deserialize<T>(string path, JsonSerializerOptions options)
        => FileEx.SafeOpenRead(
            path,
            stream =>
            {
                //corrupted files will not be valid json
                //we can try loading a backup for corrupted files
                //corrupted files will not be valid json
                //we can try loading a backup for corrupted files
                try
                {
                    JsonValidator.EnsureValidJson(stream);
                } catch (Exception e)
                {
                    throw new RetryableException("Stream content is not valid json.", e);
                }

                return JsonSerializer.Deserialize<T>(stream, options);
            });

    /// <summary>
    ///     Asynchronously deserializes a file from the specified path
    /// </summary>
    /// <param name="path">
    ///     The path to deserialize from
    /// </param>
    /// <param name="options">
    ///     The serialization options to use
    /// </param>
    /// <typeparam name="T">
    ///     The type to deserialize into
    /// </typeparam>
    /// <exception cref="IOException">
    ///     Failed to deserialize object from file, temp file, or backup file. See inner exception for details.
    /// </exception>
    public static Task<T?> DeserializeAsync<T>(string path, JsonSerializerOptions options)
        => FileEx.SafeOpenReadAsync(
            path,
            async stream =>
            {
                //corrupted files will not be valid json
                //we can try loading a backup for corrupted files
                //corrupted files will not be valid json
                //we can try loading a backup for corrupted files
                try
                {
                    JsonValidator.EnsureValidJson(stream);
                } catch (Exception e)
                {
                    throw new RetryableException("Stream content is not valid json.", e);
                }

                return await JsonSerializer.DeserializeAsync<T>(stream, options);
            });

    /// <summary>
    ///     Serializes an object to the specified path
    /// </summary>
    /// <param name="path">
    ///     The path to serialize the object to
    /// </param>
    /// <param name="value">
    ///     The object to be serialized
    /// </param>
    /// <param name="options">
    ///     The serialization options to use
    /// </param>
    /// <param name="safeSaves">
    ///     Whether or not to use atomic saves. This will produce a .bak file in the same directory as the file being saved
    /// </param>
    public static void Serialize(
        string path,
        object value,
        JsonSerializerOptions options,
        bool safeSaves = true)
    {
        var json = JsonSerializer.Serialize(value, options);

        if (safeSaves)
            FileEx.SafeWriteAllText(path, json);
        else
            File.WriteAllText(path, json);
    }

    /// <summary>
    ///     Serializes an object to the specified path
    /// </summary>
    /// <param name="path">
    ///     The path to serialize the object to
    /// </param>
    /// <param name="value">
    ///     The object to be serialized
    /// </param>
    /// <param name="options">
    ///     The serialization options to use
    /// </param>
    /// <param name="safeSaves">
    ///     Whether or not to use atomic saves. This will produce a .bak file in the same directory as the file being saved
    /// </param>
    public static Task SerializeAsync(
        string path,
        object value,
        JsonSerializerOptions options,
        bool safeSaves = true)
    {
        var json = JsonSerializer.Serialize(value, options);

        if (safeSaves)
            return FileEx.SafeWriteAllTextAsync(path, json);

        return File.WriteAllTextAsync(path, json);
    }
}