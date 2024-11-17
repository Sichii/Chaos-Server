#region
using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Messaging.Tests;

public sealed class CommandInterceptorTests
{
    private readonly CommandInterceptor<ICommandSubject, MockCommandInterceptorOptions> CommandInterceptor;
    private readonly IOptions<MockCommandInterceptorOptions> InterceptorOptions;
    private readonly Mock<ILogger<CommandInterceptor<ICommandSubject, MockCommandInterceptorOptions>>> LoggerMock;

    private readonly Mock<IServiceProvider> ServiceProviderMock;

    public CommandInterceptorTests()
    {
        InterceptorOptions = Microsoft.Extensions.Options.Options.Create(new MockCommandInterceptorOptions());
        ServiceProviderMock = MockServiceProvider.Create();
        LoggerMock = MockLogger.Create<CommandInterceptor<ICommandSubject, MockCommandInterceptorOptions>>();

        CommandInterceptor = new CommandInterceptor<ICommandSubject, MockCommandInterceptorOptions>(
            ServiceProviderMock.Object,
            InterceptorOptions,
            LoggerMock.Object);
    }

    [Test]
    public void ConstructsSuccessfully()
        => CommandInterceptor.Should()
                             .NotBeNull();

    [Test]
    public async Task HandleCommandAsync_AdminCommandWithIsAdmin_ShouldSucceed()
    {
        var commandSubjectMock = MockCommandSubject.Create("Test", true);

        await CommandInterceptor.HandleCommandAsync(commandSubjectMock.Object, "/adminCommand");

        LoggerMock.VerifyLogEvent(LogLevel.Debug, "Handling command /adminCommand");
        LoggerMock.VerifyLogEvent(LogLevel.Trace, "Successfully created command adminCommand");
        LoggerMock.VerifyLogEvent(LogLevel.Information, "ICommandSubjectProxy Test executed /adminCommand");
    }

    [Test]
    public async Task HandleCommandAsync_AdminCommandWithoutIsAdmin_ShouldFail()
    {
        var commandSubjectMock = MockCommandSubject.Create("Test", false);

        await CommandInterceptor.HandleCommandAsync(commandSubjectMock.Object, "/adminCommand");

        LoggerMock.VerifyLogEvent(LogLevel.Debug, "Handling command /adminCommand");
        LoggerMock.VerifyLogEvent(LogLevel.Warning, "Non-Admin ICommandSubjectProxy Test tried to execute admin command /adminCommand");
    }

    [Test]
    public async Task HandleCommandAsync_Exception_ShouldLog()
    {
        var commandSubjectMock = MockCommandSubject.Create("Test", true);

        await CommandInterceptor.HandleCommandAsync(commandSubjectMock.Object, "/exception");

        LoggerMock.VerifyLogEvent(LogLevel.Debug, "Handling command /exception");
        LoggerMock.VerifyLogEvent(LogLevel.Trace, "Successfully created command exception");
        LoggerMock.VerifyLogEvent(LogLevel.Error, "ICommandSubjectProxy Test failed to execute /exception", "wathapn");
    }

    [Test]
    public async Task HandleCommandAsync_HelpCommand_BuildsHelpTextCorrectly()
    {
        var commandSubjectMock = MockCommandSubject.Create("Test", true);

        await CommandInterceptor.HandleCommandAsync(commandSubjectMock.Object, "/help");

        LoggerMock.VerifyLogEvent(LogLevel.Debug, "Handling command /help");
        LoggerMock.VerifyLogEvent(LogLevel.Trace, "Successfully created command help");
        LoggerMock.VerifyLogEvent(LogLevel.Information, "ICommandSubjectProxy Test executed /help");
    }

    [Test]
    public async Task HandleCommandAsync_InvalidCommand_DoesNothing()
    {
        var commandSubjectMock = MockCommandSubject.Create("Test", true);

        await CommandInterceptor.HandleCommandAsync(commandSubjectMock.Object, "/invalidCommand");

        LoggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task HandleCommandAsync_NoCommand_DoesNothing()
    {
        var commandSubjectMock = MockCommandSubject.Create("Test", true);

        await CommandInterceptor.HandleCommandAsync(commandSubjectMock.Object, "/");

        LoggerMock.VerifyNoOtherCalls();
    }

    [Test]
    [Arguments("/test", true)]
    [Arguments("?test", false)]
    public void IsCommand_DetectsCommandCorrectly(string commandStr, bool expectedResult)
    {
        var result = CommandInterceptor.IsCommand(commandStr);

        result.Should()
              .Be(expectedResult);
    }

    [Command("adminCommand", helpText: "Admin command")]
    public sealed class AdminCommand : ICommand<ICommandSubject>
    {
        /// <inheritdoc />
        public ValueTask ExecuteAsync(ICommandSubject source, ArgumentCollection args) => ValueTask.CompletedTask;
    }

    [Command("exception", helpText: "???")]
    public sealed class ExceptionCommand : ICommand<ICommandSubject>
    {
        /// <inheritdoc />
        public ValueTask ExecuteAsync(ICommandSubject source, ArgumentCollection args) => throw new Exception("wathapn");
    }

    [Command("help", helpText: "This help text")]
    public sealed class HelpCommand : ICommand<ICommandSubject>
    {
        /// <inheritdoc />
        public ValueTask ExecuteAsync(ICommandSubject source, ArgumentCollection args)
        {
            args.Should()
                .HaveCount(1);
            var helpText = args.First();

            helpText.Should()
                    .ContainAll(
                        "adminCommand",
                        "Admin command",
                        "help",
                        "This help text",
                        "exception",
                        "???");

            return default;
        }
    }

    public sealed class MockCommandInterceptorOptions : ICommandInterceptorOptions
    {
        /// <inheritdoc />
        public string Prefix { get; set; } = "/";
    }
}