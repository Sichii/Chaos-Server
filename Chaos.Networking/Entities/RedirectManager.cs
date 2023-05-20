using Chaos.Networking.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Chaos.Networking.Entities;

/// <summary>
///     Represents an object used to manage redirects
/// </summary>
public sealed class RedirectManager : BackgroundService, IRedirectManager
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
    private readonly ILogger<RedirectManager> Logger;
    private readonly ConcurrentDictionary<uint, IRedirect> Redirects = new();

    /// <summary>
    ///     Initializes a new instance of <see cref="RedirectManager" />
    /// </summary>
    public RedirectManager(ILogger<RedirectManager> logger) => Logger = logger;

    /// <inheritdoc />
    public void Add(IRedirect redirect)
    {
        Logger.LogTrace("Now tracking {@Redirect}", redirect);
        Redirects.TryAdd(redirect.Id, redirect);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                var now = DateTime.UtcNow;

                foreach (var redirect in Redirects.Values)
                    if (now.Subtract(redirect.Created) > Timeout)
                    {
                        Logger.LogTrace("{@Redirect} has timed out", redirect);
                        Redirects.TryRemove(redirect.Id, out _);
                    }
            } catch (OperationCanceledException)
            {
                return;
            }
    }

    /// <inheritdoc />
    public bool TryGetRemove(uint id, [MaybeNullWhen(false)] out IRedirect redirect)
    {
        if (Redirects.TryRemove(id, out redirect))
        {
            Logger.LogTrace("{@Redirect} has been consumed", redirect);

            return true;
        }

        return false;
    }
}