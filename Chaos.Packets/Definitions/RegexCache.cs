using System.Text.RegularExpressions;

namespace Chaos.Packets.Definitions;

internal static partial class RegexCache
{
    internal static readonly Regex DOUBLE_BYTE_REGEX = GenerateDoubleByteRegex();

    [RegexGenerator("(.{2})", RegexOptions.Compiled)]
    private static partial Regex GenerateDoubleByteRegex();
}