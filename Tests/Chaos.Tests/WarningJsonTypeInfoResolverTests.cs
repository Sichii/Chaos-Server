#region
using System.Text.Json;
using Chaos.Utilities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class WarningJsonTypeInfoResolverTests
{
    private readonly Mock<ILogger<WarningJsonTypeInfoResolver>> LoggerMock;
    private readonly JsonSerializerOptions Options;
    private readonly WarningJsonTypeInfoResolver Resolver;

    public WarningJsonTypeInfoResolverTests()
    {
        LoggerMock = new Mock<ILogger<WarningJsonTypeInfoResolver>>();

        LoggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>()))
                  .Returns(true);

        Resolver = new WarningJsonTypeInfoResolver(LoggerMock.Object);
        Options = new JsonSerializerOptions();
    }

    #region GetTypeInfo
    [Test]
    public void GetTypeInfo_ConcreteType_ShouldLogTrace()
    {
        var result = Resolver.GetTypeInfo(typeof(string), Options);

        result.Should()
              .NotBeNull();

        LoggerMock.Verify(
            l => l.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void GetTypeInfo_Interface_ShouldNotLog()
    {
        var result = Resolver.GetTypeInfo(typeof(IDisposable), Options);

        result.Should()
              .NotBeNull();

        LoggerMock.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Test]
    public void GetTypeInfo_AbstractType_ShouldNotLog()
    {
        var result = Resolver.GetTypeInfo(typeof(Stream), Options);

        result.Should()
              .NotBeNull();

        LoggerMock.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Test]
    public void GetTypeInfo_GenericTypeDefinition_ShouldNotLog()
    {
        // base.GetTypeInfo throws for open generic types, but the guard should prevent logging
        var act = () => Resolver.GetTypeInfo(typeof(List<>), Options);

        act.Should()
           .Throw<InvalidOperationException>();

        LoggerMock.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
    #endregion
}