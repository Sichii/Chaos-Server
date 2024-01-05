using Chaos.Collections.Synchronized;

namespace Chaos.IO.FileSystem;

/// <summary>
///     Provides methods for performing synchronized operations on directories
/// </summary>
public static class DirectorySynchronizer
{
    internal static readonly SynchronizedHashSet<string> LockedDirectories;

    static DirectorySynchronizer() => LockedDirectories = new SynchronizedHashSet<string>(comparer: StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Executes the specified function on the specified directory, ensuring that no other actions are being performed on
    ///     the directory
    /// </summary>
    /// <param name="directory">
    ///     The directory to lock during execution
    /// </param>
    /// <param name="action">
    ///     The function to execute
    /// </param>
    public static void SafeExecute(this string directory, Action<string> action)
    {
        SpinWait.SpinUntil(() => LockedDirectories.Add(directory));

        try
        {
            action(directory);
        } finally
        {
            LockedDirectories.Remove(directory);
        }
    }

    /// <summary>
    ///     Executes the specified function on the specified directory, ensuring that no other actions are being performed on
    ///     the directory
    /// </summary>
    /// <param name="directory">
    ///     The directory to lock during execution
    /// </param>
    /// <param name="function">
    ///     The function to execute
    /// </param>
    public static TResult SafeExecute<TResult>(this string directory, Func<string, TResult> function)
    {
        SpinWait.SpinUntil(() => LockedDirectories.Add(directory));

        try
        {
            return function(directory);
        } finally
        {
            LockedDirectories.Remove(directory);
        }
    }

    /// <summary>
    ///     Asynchronously executes the specified function on the specified directory, ensuring that no other actions are being
    ///     performed on the directory
    /// </summary>
    /// <param name="directory">
    ///     The directory to lock during execution
    /// </param>
    /// <param name="function">
    ///     The function to execute
    /// </param>
    public static async Task SafeExecuteAsync(this string directory, Func<string, Task> function)
    {
        while (!LockedDirectories.Add(directory))
            await Task.Yield();

        try
        {
            await function(directory);
        } finally
        {
            LockedDirectories.Remove(directory);
        }
    }

    /// <summary>
    ///     Asynchronously executes the specified function on the specified directory, ensuring that no other actions are being
    ///     performed on the directory
    /// </summary>
    /// <param name="directory">
    ///     The directory to lock during execution
    /// </param>
    /// <param name="function">
    ///     The function to execute
    /// </param>
    public static async Task<T> SafeExecuteAsync<T>(this string directory, Func<string, Task<T>> function)
    {
        while (!LockedDirectories.Add(directory))
            await Task.Yield();

        try
        {
            return await function(directory);
        } finally
        {
            LockedDirectories.Remove(directory);
        }
    }
}