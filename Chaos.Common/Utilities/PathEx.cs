namespace Chaos.Common.Utilities;

public static class PathEx
{
    public static bool IsSubPathOf(string path, string parentPath)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (string.IsNullOrEmpty(parentPath))
            throw new ArgumentNullException(nameof(parentPath));

        var pathParts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var parentPathParts = parentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (pathParts.Length < parentPathParts.Length)
            return false;

        for (var i = 0; i < parentPathParts.Length; i++)
            if (!parentPathParts[i].Equals(pathParts[i], StringComparison.OrdinalIgnoreCase))
                return false;

        return true;
    }
}