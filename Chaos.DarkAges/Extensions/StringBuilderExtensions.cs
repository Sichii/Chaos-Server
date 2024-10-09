using System.Text;
using Chaos.DarkAges.Definitions;

namespace Chaos.DarkAges.Extensions;

/// <summary>
///     Extensions for <see cref="StringBuilder" />
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    ///     Appends the color prefix to the <see cref="StringBuilder" /> followed by the message and then the default color
    /// </summary>
    /// <param name="builder">
    ///     This stringbuilder
    /// </param>
    /// <param name="message">
    ///     The message to append
    /// </param>
    /// <param name="messageColor">
    ///     The color of the message
    /// </param>
    /// <param name="defaultColor">
    ///     The color to change back to at the end of the message
    /// </param>
    public static StringBuilder AppendColored(
        this StringBuilder builder,
        MessageColor messageColor,
        string message,
        MessageColor? defaultColor = null)
    {
        builder.AppendColorPrefix(messageColor);
        builder.Append(message);

        if (defaultColor.HasValue)
            builder.AppendColorPrefix(defaultColor.Value);

        return builder;
    }

    /// <summary>
    ///     Appends the color prefix to the <see cref="StringBuilder" />
    /// </summary>
    /// <param name="builder">
    ///     This stringbuilder
    /// </param>
    /// <param name="color">
    ///     The color to change the following text to
    /// </param>
    /// <returns>
    /// </returns>
    public static StringBuilder AppendColorPrefix(this StringBuilder builder, MessageColor color)
    {
        builder.Append(color.ToPrefix());

        return builder;
    }

    /// <summary>
    ///     Appends a formatted string to the end of the <see cref="StringBuilder" /> followed by a linefeed
    /// </summary>
    /// <param name="builder">
    ///     This stringbuilder
    /// </param>
    /// <param name="value">
    ///     The string to append before a newline
    /// </param>
    public static StringBuilder AppendLineF(this StringBuilder builder, string? value)
    {
        builder.Append(value);
        builder.Append('\n');

        return builder;
    }

    /// <summary>
    ///     Appends the color prefix to the <see cref="StringBuilder" /> followed by the message, line feed, and then the
    ///     default color
    /// </summary>
    /// <param name="builder">
    ///     This stringbuilder
    /// </param>
    /// <param name="message">
    ///     The message to append
    /// </param>
    /// <param name="messageColor">
    ///     The color of the message
    /// </param>
    /// <param name="defaultColor">
    ///     The color to change back to at the end of the message
    /// </param>
    public static StringBuilder AppendLineFColored(
        this StringBuilder builder,
        MessageColor messageColor,
        string message,
        MessageColor? defaultColor = null)
    {
        builder.AppendColorPrefix(messageColor);
        builder.AppendLineF(message);

        if (defaultColor.HasValue)
            builder.AppendColorPrefix(defaultColor.Value);

        return builder;
    }
}