using System.Runtime.InteropServices;

namespace ChaosTool.Comparers;

public sealed class NaturalStringComparer : IComparer<string>
{
    public static IComparer<string> Instance { get; } = new NaturalStringComparer();

    public int Compare(string? x, string? y)
    {
        if (x is null || y is null)
            return StringComparer.OrdinalIgnoreCase.Compare(x, y);

        return StrCmpLogicalW(x, y);
    }

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    #pragma warning disable SYSLIB1054
    private static extern int StrCmpLogicalW(string x, string y);
    #pragma warning restore SYSLIB1054
}