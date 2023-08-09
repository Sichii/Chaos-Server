using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
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
        Logger.WithTopics(Topics.Actions.Redirect)
              .LogTrace("Now tracking redirect {@RedirectId}", redirect.Id);

        Redirects.TryAdd(redirect.Id, redirect);
    }

    /// <inheritdoc />
    public bool TryGetRemove(uint id, [MaybeNullWhen(false)] out IRedirect redirect)
    {
        if (Redirects.TryRemove(id, out redirect))
        {
            Logger.WithTopics(Topics.Actions.Redirect)
                  .LogTrace("Redirect {@RedirectId} has been consumed", redirect.Id);

            return true;
        }

        return false;
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
                        Logger.WithTopics(Topics.Actions.Redirect)
                              .LogTrace("Redirect {@RedirectId} has timed out", redirect.Id);

                        Redirects.TryRemove(redirect.Id, out _);
                    }
            } catch (OperationCanceledException)
            {
                return;
            }
    }
}