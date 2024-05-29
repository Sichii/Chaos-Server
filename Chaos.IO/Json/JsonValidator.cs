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
    public static bool IsValidJson(Stream jsonStream)
    {
        var position = jsonStream.Position;
        Span<byte> buffer = stackalloc byte[4096];
        var state = new JsonReaderState();

        try
        {
            int bytesRead;

            while ((bytesRead = jsonStream.Read(buffer)) > 0)
            {
                var jsonReader = new Utf8JsonReader(buffer[..bytesRead], bytesRead < buffer.Length, state);

                while (jsonReader.Read())
                {
                    // Intentionally empty loop to read through the entire JSON
                }

                // Save the state for the next iteration
                state = jsonReader.CurrentState;
            }

            jsonStream.Position = position;

            return true;
        } catch (JsonException)
        {
            jsonStream.Position = position;

            return false;
        }
    }
}