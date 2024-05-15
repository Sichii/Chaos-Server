using System.Text;

namespace Chaos.IO.FileSystem;

/// <summary>
///     A static utility for performing file operations
/// </summary>
public static class FileEx
{
    /// <summary>
    ///     Writes all text to a file. Corruption will not occur if the write fails.
    /// </summary>
    /// <param name="path">
    ///     The path to write the text to
    /// </param>
    /// <param name="text">
    ///     The text to write
    /// </param>
    public static void SafeWriteAllText(string path, string text)
    {
        var tries = 0;

        while (tries < 3)
        {
            var tempPath = path + ".temp";
            var bakPath = path + ".bak";

            File.WriteAllText(tempPath, text);

            if (File.Exists(path))
                File.Replace(tempPath, path, bakPath);
            else
                File.Move(tempPath, path, true);

            if (File.Exists(path))
                return;

            tries++;

            Thread.Sleep(50 + tries * 50);
        }

        var builder = new StringBuilder();
        builder.AppendLine($"Failed to write text to file \"{path}\"");
        builder.AppendLine(text);

        throw new IOException(builder.ToString());
    }

    /// <summary>
    ///     Asynchronously writes all text to a file. Corruption will not occur if the write fails.
    /// </summary>
    /// <param name="path">
    ///     The path to write the text to
    /// </param>
    /// <param name="text">
    ///     The text to write
    /// </param>
    public static async Task SafeWriteAllTextAsync(string path, string text)
    {
        var tries = 0;

        while (tries < 3)
        {
            var tempPath = path + ".temp";
            var bakPath = path + ".bak";

            await File.WriteAllTextAsync(tempPath, text);

            if (File.Exists(path))
                File.Replace(
                    tempPath,
                    path,
                    bakPath,
                    true);
            else
                File.Move(tempPath, path, true);

            if (File.Exists(path))
                return;

            tries++;

            await Task.Delay(50 + tries * 50);
        }

        var builder = new StringBuilder();
        builder.AppendLine($"Failed to write text to file \"{path}\"");
        builder.AppendLine(text);

        throw new IOException(builder.ToString());
    }
}