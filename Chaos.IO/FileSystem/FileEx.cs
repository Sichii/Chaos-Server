using System.Text;
using Chaos.IO.Exceptions;

namespace Chaos.IO.FileSystem;

/// <summary>
///     A static utility for performing file operations
/// </summary>
public static class FileEx
{
    /// <summary>
    ///     Safely opens a file for reading and performs the specified action
    /// </summary>
    /// <param name="path">
    ///     The path to open for reading
    /// </param>
    /// <param name="func">
    ///     The func to perform on the stream
    /// </param>
    /// <typeparam name="T">
    ///     The type to return from the func provided
    /// </typeparam>
    /// <exception cref="IOException">
    ///     Failed to deserialize object from file, temp file, or backup file. See inner exception for details.
    /// </exception>
    public static T SafeOpenRead<T>(string path, Func<FileStream, T> func)
    {
        var dir = Path.GetDirectoryName(path)!;
        var fileName = Path.GetFileName(path);
        var tempPath = Path.Combine(dir, fileName + ".temp");
        var backPath = Path.Combine(dir, fileName + ".bak");

        var pathsToTry = new[]
        {
            path,
            tempPath,
            backPath
        };
        List<Exception> exceptions = [];

        foreach (var pathToTry in pathsToTry)
            try
            {
                using var stream = File.Open(
                    pathToTry,
                    new FileStreamOptions
                    {
                        Access = FileAccess.Read,
                        Mode = FileMode.Open,
                        Options = FileOptions.SequentialScan,
                        Share = FileShare.ReadWrite
                    });

                var ret = func(stream);

                return ret;
            } catch (FileNotFoundException e)
            {
                exceptions.Add(e);
            } catch (RetryableException e)
            {
                exceptions.Add(e);
            } catch (Exception e)
            {
                exceptions.Add(e);

                break;
            }

        throw new AggregateException("Failed to read file, temp file, or backup file. See inner exceptions for details.", exceptions);
    }

    /// <summary>
    ///     Safely opens a file for reading and performs the specified action
    /// </summary>
    /// <param name="path">
    ///     The path to open for reading
    /// </param>
    /// <param name="func">
    ///     The func to perform on the stream
    /// </param>
    /// <typeparam name="T">
    ///     The type to return from the func provided
    /// </typeparam>
    /// <exception cref="IOException">
    ///     Failed to deserialize object from file, temp file, or backup file. See inner exception for details.
    /// </exception>
    public static async Task<T> SafeOpenReadAsync<T>(string path, Func<FileStream, Task<T>> func)
    {
        var dir = Path.GetDirectoryName(path)!;
        var fileName = Path.GetFileName(path);
        var tempPath = Path.Combine(dir, fileName + ".temp");
        var backPath = Path.Combine(dir, fileName + ".bak");

        var pathsToTry = new[]
        {
            path,
            tempPath,
            backPath
        };
        List<Exception> exceptions = [];

        foreach (var pathToTry in pathsToTry)
            try
            {
                await using var stream = File.Open(
                    pathToTry,
                    new FileStreamOptions
                    {
                        Access = FileAccess.Read,
                        Mode = FileMode.Open,
                        Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
                        Share = FileShare.ReadWrite
                    });

                var ret = await func(stream);

                return ret;
            } catch (FileNotFoundException e)
            {
                exceptions.Add(e);
            } catch (RetryableException re)
            {
                exceptions.Add(re);
            } catch (Exception e)
            {
                exceptions.Add(e);

                break;
            }

        throw new AggregateException("Failed to read file, temp file, or backup file. See inner exceptions for details.", exceptions);
    }

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