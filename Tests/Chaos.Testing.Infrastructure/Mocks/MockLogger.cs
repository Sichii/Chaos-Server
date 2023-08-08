using Chaos.Extensions.Common;
using Chaos.NLog.Logging;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockLogger
{
    public static Mock<ILogger<T>> Create<T>(LogLevel? minLevel = LogLevel.Trace, Action<Mock<ILogger<T>>>? setup = null)
    {
        var mock = new Mock<ILogger<T>>();
        mock.Setup(l => l.IsEnabled(It.Is<LogLevel>(ll => ll >= minLevel)));
        setup?.Invoke(mock);

        return mock;
    }

    private static bool VerifyException(Exception? e, Exception? expected) => VerifyException(e, expected?.Message);

    private static bool VerifyException(Exception? e, string? expectedMessage)
    {
        if (e is null)
            return expectedMessage is null;

        if (expectedMessage is null)
            return false;

        return e.Message.EqualsI(expectedMessage);
    }

    public static void VerifyLogEvent<T>(
        this Mock<ILogger<T>> mockLogger,
        LogLevel logLevel,
        string message,
        string? exceptionMessage = null,
        Times? times = null,
        params object[] args
    ) =>
        mockLogger.Verify(
            l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<LogEvent>(e => !args.Any() || args.All(arg => e.Select(kvp => kvp.Value).Contains(arg))),
                It.Is<Exception>(e => VerifyException(e, exceptionMessage)),
                It.Is<Func<LogEvent, Exception?, string>>(f => f(null!, null) == message)),
            times ?? Times.Once());

    public static void VerifyLogEvent<T>(
        this Mock<ILogger<T>> mockLogger,
        LogLevel logLevel,
        string message,
        Exception? exception,
        Times? times = null,
        params object[] args
    ) =>
        mockLogger.Verify(
            l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<LogEvent>(e => !args.Any() || args.All(arg => e.Select(kvp => kvp.Value).Contains(arg))),
                It.Is<Exception>(e => VerifyException(e, exception)),
                It.Is<Func<LogEvent, Exception?, string>>(f => f(null!, exception) == message)),
            times ?? Times.Once());

    public static void VerifySimpleLog<T>(
        this Mock<ILogger<T>> mockLogger,
        LogLevel logLevel,
        string message,
        string? exceptionMessage = null,
        Times? times = null
    )
        => mockLogger.Verify(
            l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString() == message),
                It.Is<Exception?>(e => VerifyException(e, exceptionMessage)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once());
}