namespace Chaos.IO.FileSystem;

/// <summary>
///     Provides utility methods for working with file system paths.
/// </summary>
public static class PathEx
{
    /// <summary>
    ///     Determines whether the specified path is a subpath of the given parent path.
    /// </summary>
    /// <param name="path">
    ///     The path to check if it's a subpath.
    /// </param>
    /// <param name="parentPath">
    ///     The parent path to compare against.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the specified path is a subpath of the parent path; otherwise,
    ///     <c>
    ///         false
    ///     </c>
    ///     .
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when either <paramref name="path" /> or <paramref name="parentPath" /> is
    ///     <c>
    ///         null
    ///     </c>
    ///     or empty.
    /// </exception>
    /// <remarks>
    ///     This method performs a case-insensitive comparison of the path parts.
    /// </remarks>
    public static bool IsSubPathOf(string path, string parentPath)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (string.IsNullOrEmpty(parentPath))
            throw new ArgumentNullException(nameof(parentPath));

        var pathSpan = Path.GetFullPath(path)
                           .AsSpan();

        var parentPathSpan = Path.GetFullPath(parentPath)
                                 .AsSpan();

        var pathParts = pathSpan.SplitAny(
            [
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            ]);

        var parentPathParts = parentPathSpan.SplitAny(
            [
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            ]);

        while (parentPathParts.MoveNext())
            if (pathParts.MoveNext())
            {
                var parentPathPart = parentPathSpan[parentPathParts.Current];
                var pathPart = pathSpan[pathParts.Current];

                if (!pathPart.Equals(parentPathPart, StringComparison.OrdinalIgnoreCase))
                    return false;
            } else
                return false;

        return true;
    }
}