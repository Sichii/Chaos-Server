namespace Chaos.IO.FileSystem;

/// <summary>
///     Provides methods for performing synchronized operations on directories
/// </summary>
public static class DirectorySynchronizer
{
    internal static readonly HashSet<string> LockedPaths;
    private static readonly object Sync = new();

    static DirectorySynchronizer() => LockedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private static bool InnerTryAddPath(string innerPath)
    {
        //lock LockedPaths
        lock (Sync)
        {
            if (LockedPaths.Any(lockedPath => PathEx.IsSubPathOf(innerPath, lockedPath)))
                return false;

            return LockedPaths.Add(innerPath);
        }
    }

    private static void LockPath(string path) => SpinWait.SpinUntil(() => InnerTryAddPath(path));

    private static async Task LockPathAsync(string path)
    {
        while (!InnerTryAddPath(path))
            await Task.Yield();
    }

    /// <param name="path">
    ///     The directory to lock during execution
    /// </param>
    extension(string path)
    {
        /// <summary>
        ///     Executes the specified function on the specified directory, ensuring that no other actions are being performed on
        ///     the directory
        /// </summary>
        /// <param name="action">
        ///     The function to execute
        /// </param>
        public void SafeExecute(Action<string> action)
        {
            LockPath(path);

            try
            {
                action(path);
            } finally
            {
                lock (Sync)
                    LockedPaths.Remove(path);
            }
        }

        /// <summary>
        ///     Executes the specified function on the specified directory, ensuring that no other actions are being performed on
        ///     the directory
        /// </summary>
        /// <param name="function">
        ///     The function to execute
        /// </param>
        public TResult SafeExecute<TResult>(Func<string, TResult> function)
        {
            LockPath(path);

            try
            {
                return function(path);
            } finally
            {
                lock (Sync)
                    LockedPaths.Remove(path);
            }
        }

        /// <summary>
        ///     Asynchronously executes the specified function on the specified path, ensuring that no other actions are being
        ///     performed on the path, or any sub-paths
        /// </summary>
        /// <param name="function">
        ///     The function to execute
        /// </param>
        public async Task SafeExecuteAsync(Func<string, Task> function)
        {
            await LockPathAsync(path);

            try
            {
                await function(path);
            } finally
            {
                lock (Sync)
                    LockedPaths.Remove(path);
            }
        }

        /// <summary>
        ///     Asynchronously executes the specified function on the specified path, ensuring that no other actions are being
        ///     performed on the path, or any sub-paths
        /// </summary>
        /// <param name="function">
        ///     The function to execute
        /// </param>
        public async Task<T> SafeExecuteAsync<T>(Func<string, Task<T>> function)
        {
            await LockPathAsync(path);

            try
            {
                return await function(path);
            } finally
            {
                lock (Sync)
                    LockedPaths.Remove(path);
            }
        }
    }
}