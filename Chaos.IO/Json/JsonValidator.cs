using System.Text.Json;

namespace Chaos.IO.Json;

/// <summary>
///     A static utility for validating JSON
/// </summary>
public static class JsonValidator
{
    /// <summary>
    ///     Determines if the specified stream is valid JSON
    /// </summary>
    /// <param name="jsonStream">
    ///     The stream to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the stream is valid JSON; otherwise,
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static void EnsureValidJson(Stream jsonStream)
    {
        var position = jsonStream.Position;
        var buffer = new byte[jsonStream.Length];

        _ = jsonStream.Read(buffer);

        try
        {
            var jsonReader = new Utf8JsonReader(buffer);

            while (jsonReader.Read())
            {
                // Intentionally empty loop to read through the entire JSON
            }
        } finally
        {
            jsonStream.Position = position;
        }
    }
}