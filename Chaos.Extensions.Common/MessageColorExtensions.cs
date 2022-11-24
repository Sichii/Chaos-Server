using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="MessageColor" />.
/// </summary>
public static class MessageColorExtensions
{
    /// <summary>
    ///     Converts the <see cref="MessageColor"/> to it's in-game "{=" prefix
    /// </summary>
    /// <param name="messageColor">The <see cref="MessageColor"/> to convert</param>
    /// <returns>The string prefix required to change the color of a message in-game</returns>
    public static string ToPrefix(this MessageColor messageColor) => $"{{={(char)messageColor}";
}