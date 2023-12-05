using Chaos.Common.Synchronization;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class AutoReleasingMonitorTests
{
    [Fact]
    public void Enter_ShouldAcquireLockAndReleaseOnDispose()
    {
        // Arrange
        var monitor = new AutoReleasingMonitor();
        bool lockAcquired;

        // Act
        using (monitor.Enter())
            lockAcquired = true;

        // Assert
        lockAcquired.Should()
                    .BeTrue();

        monitor.IsEntered
               .Should()
               .BeFalse();
    }

    [Fact]
    public void EnterWithSafeExit_ShouldAcquireLockAndReleaseOnDispose()
    {
        // Arrange
        var monitor = new AutoReleasingMonitor();
        bool lockAcquired;

        // Act
        using (monitor.EnterWithSafeExit())
            lockAcquired = true;

        // Assert
        lockAcquired.Should()
                    .BeTrue();

        monitor.IsEntered
               .Should()
               .BeFalse();
    }

    [Fact]
    public void Exit_ShouldReleaseLock()
    {
        // Arrange
        var monitor = new AutoReleasingMonitor();
        monitor.Enter();

        // Act
        monitor.Exit();

        // Assert
        monitor.IsEntered
               .Should()
               .BeFalse();
    }

    [Fact]
    public void NoOpDisposable_ShouldDoNothingOnDispose()
    {
        // Arrange
        var disposable = AutoReleasingMonitor.NoOpDisposable;

        // Act
        var disposeAction = () => disposable.Dispose();

        // Assert
        disposeAction.Should()
                     .NotThrow();
    }

    [Fact]
    public void TryEnter_ShouldAcquireLockAndReleaseOnDispose()
    {
        // Arrange
        var monitor = new AutoReleasingMonitor();
        bool lockAcquired;

        // Act
        using (monitor.TryEnter(100))
            lockAcquired = true;

        // Assert
        lockAcquired.Should()
                    .BeTrue();

        monitor.IsEntered
               .Should()
               .BeFalse();
    }

    [Fact]
    public void TryEnter_ShouldReturnNullWhenFailedToAcquireLock()
    {
        // Arrange
        var monitor = new AutoReleasingMonitor();

        // Act
        using var @lock = monitor.Enter();
        IDisposable? subscription = null!;
        var executed = ThreadPool.QueueUserWorkItem(_ => subscription = monitor.TryEnter(100));

        // Assert
        executed.Should()
                .BeTrue();

        subscription.Should()
                    .BeNull();
    }
}