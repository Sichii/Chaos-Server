using System.Text;

namespace Chaos.Common.Utilities;

/// <summary>
///     A static utility for performing file operations
/// </summary>
public static class FileEx
{
    /// <summary>
    ///     Writes all text to a file. Corruption will not occur if the write fails.
    /// </summary>
    /// <param name="path">The path to write the text to</param>
    /// <param name="text">The text to write</param>
    /// <param name="createBackup">Whether or not to take a backup if a file already exists at the given path</param>
    public static void SafeWriteAllText(string path, string text, bool createBackup = false)
    {
        var tempPath = path + ".temp";
        var bakPath = path + ".bak";

        using (var stream = File.Create(tempPath))
            stream.Write(Encoding.UTF8.GetBytes(text));

        if (File.Exists(path))
            File.Replace(tempPath, path, createBackup ? bakPath : null);
        else
            File.Move(tempPath, path);
    }

    /// <summary>
    ///     Asynchronously writes all text to a file. Corruption will not occur if the write fails.
    /// </summary>
    /// <param name="path">The path to write the text to</param>
    /// <param name="text">The text to write</param>
    /// <param name="createBackup">Whether or not to take a backup if a file already exists at the given path</param>
    public static async Task SafeWriteAllTextAsync(string path, string text, bool createBackup = false)
    {
        var tempPath = path + ".temp";
        var bakPath = path + ".bak";

        await using (var stream = File.Create(tempPath))
            await stream.WriteAsync(Encoding.UTF8.GetBytes(text));

        if (File.Exists(path))
            File.Replace(tempPath, path, createBackup ? bakPath : null);
        else
            File.Move(tempPath, path);
    }
}