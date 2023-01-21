using System.Text;

// ReSharper disable ConvertIfStatementToSwitchStatement

namespace Chaos.Extensions.Common;

public static class TimeSpanExtensions
{
    public static string ToReadableString(
        this TimeSpan timeSpan,
        bool showMilliseconds = false,
        bool showSeconds = true,
        bool showMinutes = true,
        bool showHours = true,
        bool showDays = true
    )
    {
        var sb = new StringBuilder();

        if (showDays)
            if (timeSpan.Days > 1)
                sb.Append($"{timeSpan.Days} days ");
            else if (timeSpan.Days > 0)
                sb.Append($"{timeSpan.Days} day ");

        if (showHours)
            if (timeSpan.Hours > 1)
                sb.Append($"{timeSpan.Hours} hours ");
            else if (timeSpan.Hours > 0)
                sb.Append($"{timeSpan.Hours} hour ");

        if (showMinutes)
            if (timeSpan.Minutes > 1)
                sb.Append($"{timeSpan.Minutes} mins ");
            else if (timeSpan.Minutes > 0)
                sb.Append($"{timeSpan.Minutes} min ");

        if (showSeconds)
            if (timeSpan.Seconds > 1)
                sb.Append($"{timeSpan.Seconds} secs ");
            else if (timeSpan.Seconds > 0)
                sb.Append($"{timeSpan.Seconds} sec ");

        if (showMilliseconds)
            if (timeSpan.Milliseconds > 0)
                sb.Append($"{timeSpan.Milliseconds}ms ");

        return sb.ToString().Trim();
    }
}