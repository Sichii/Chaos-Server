using Microsoft.Extensions.Logging;

namespace Chaos.Core.Extensions;

public static class LoggerExtensions
{
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