using Microsoft.Extensions.Logging;

namespace Chaos.Core.Extensions;

public static class LoggerExtensions
{
    /// <summary>
    ///     Logs an error with an object as a data context. The object is converted to a string and logged.
    /// </summary>
    public static void LogErrorWithContext(
        this ILogger logger,
        string message,
        object context,
        object stackTrace
    ) =>
        logger.LogError(
            "{Message}{Context}{StackTrace}",
            message,
            context.ToStringSynthetic(),
            stackTrace.ToString());
}