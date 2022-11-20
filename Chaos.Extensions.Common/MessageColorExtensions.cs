using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

public static class MessageColorExtensions
{
    public static string ToPrefix(this MessageColor messageColor) => $"{{={(char)messageColor}";
}