#region
using System.Net;
using System.Reflection;
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Entities;
using Chaos.Networking.Options;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class RedirectManagerTests : IDisposable
{
    private readonly RedirectManager Manager;

    public RedirectManagerTests()
    {
        var logger = MockLogger.Create<RedirectManager>();
        Manager = new RedirectManager(logger.Object);
    }

    public void Dispose() => Manager.Dispose();

    [Test]
    public void Add_DuplicateId_ShouldNotOverwrite()
    {
        var redirect1 = CreateRedirect(1);
        var redirect2 = CreateRedirect(1);

        Manager.Add(redirect1);
        Manager.Add(redirect2);

        Manager.TryGetRemove(1, out var retrieved);

        // ConcurrentDictionary.TryAdd returns false for duplicates, so original stays
        retrieved.Should()
                 .BeSameAs(redirect1);
    }

    [Test]
    public void Add_MultipleRedirects_ShouldTrackAllIndependently()
    {
        var redirect1 = CreateRedirect(1);
        var redirect2 = CreateRedirect(2);
        var redirect3 = CreateRedirect(3);

        Manager.Add(redirect1);
        Manager.Add(redirect2);
        Manager.Add(redirect3);

        Manager.TryGetRemove(2, out var retrieved)
               .Should()
               .BeTrue();

        retrieved.Should()
                 .BeSameAs(redirect2);

        // Others should still be available
        Manager.TryGetRemove(1, out _)
               .Should()
               .BeTrue();

        Manager.TryGetRemove(3, out _)
               .Should()
               .BeTrue();
    }

    [Test]
    public void Add_ThenTryGetRemove_ShouldReturnRedirect()
    {
        var redirect = CreateRedirect(1);

        Manager.Add(redirect);
        var result = Manager.TryGetRemove(1, out var retrieved);

        result.Should()
              .BeTrue();

        retrieved.Should()
                 .BeSameAs(redirect);
    }

    private static Redirect CreateRedirect(uint id)
    {
        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.Loopback,
            Port = 4200
        };

        return new Redirect(
            id,
            serverInfo,
            ServerType.Login,
            "key",
            0);
    }

    [Test]
    public async Task ExecuteAsync_ShouldNotRemoveNonExpiredRedirects()
    {
        var redirect = CreateRedirect(200);

        Manager.Add(redirect);

        using var cts = new CancellationTokenSource();
        var executeTask = StartExecuteAsync(cts.Token);

        // Wait for the periodic timer to fire
        await Task.Delay(1500);

        var result = Manager.TryGetRemove(200, out var retrieved);

        result.Should()
              .BeTrue("non-expired redirect should still be available");

        retrieved.Should()
                 .BeSameAs(redirect);

        await cts.CancelAsync();

        try
        {
            await executeTask;
        } catch (OperationCanceledException) { }
    }

    [Test]
    public async Task ExecuteAsync_ShouldRemoveExpiredRedirects()
    {
        // Add a redirect and then use reflection to set Created to past
        var redirect = CreateRedirect(100);

        Manager.Add(redirect);

        // Use reflection to set Created to 15 seconds ago (timeout is 10 seconds)
        var createdField = typeof(Redirect).GetField("<Created>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!;

        createdField.SetValue(redirect, DateTime.UtcNow.AddSeconds(-15));

        // Start the manager as a background service
        using var cts = new CancellationTokenSource();
        var executeTask = StartExecuteAsync(cts.Token);

        // Wait for the periodic timer to fire and clean up
        await Task.Delay(1500);

        var result = Manager.TryGetRemove(100, out _);

        result.Should()
              .BeFalse("redirect should have been removed by timeout cleanup");

        await cts.CancelAsync();

        try
        {
            await executeTask;
        } catch (OperationCanceledException) { }
    }

    private Task StartExecuteAsync(CancellationToken ct)
    {
        // Use reflection to invoke protected ExecuteAsync
        var method = typeof(RedirectManager).GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;

        return (Task)method.Invoke(Manager, [ct])!;
    }

    [Test]
    public void TryGetRemove_ShouldConsumeRedirect_SoSecondCallReturnsFalse()
    {
        var redirect = CreateRedirect(1);

        Manager.Add(redirect);
        Manager.TryGetRemove(1, out _);
        var result = Manager.TryGetRemove(1, out _);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryGetRemove_ShouldReturnFalse_WhenRedirectDoesNotExist()
    {
        var result = Manager.TryGetRemove(99, out var retrieved);

        result.Should()
              .BeFalse();

        retrieved.Should()
                 .BeNull();
    }
}