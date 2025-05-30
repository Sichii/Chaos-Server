#region
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions;
using Chaos.Time;
using Humanizer;
using StringExtensions = Chaos.Extensions.Common.StringExtensions;
#endregion

namespace Chaos.Utilities;

public static class ShutdownUtility
{
    private const string SHUTDOWN_FORMAT = "Server will be shutting down in {Time}";
    private static readonly Lock Sync;
    public static CancellationTokenSource? CancellationTokenSource { get; private set; }
    public static Task? ShutdownTask { get; private set; }

    static ShutdownUtility() => Sync = new Lock();

    public static void AbortShutdown() => CancellationTokenSource?.Cancel();

    public static void BeginShutdown(IServiceProvider serviceProvider, int mins)
    {
        using var @lock = Sync.EnterScope();

        if (ShutdownTask != null)
            throw new InvalidOperationException("Shutdown already in progress");

        var clientRegistry = serviceProvider.GetRequiredService<IClientRegistry<IChaosWorldClient>>();
        var serverCancellationTokenSource = serviceProvider.GetRequiredService<CancellationTokenSource>();

        CancellationTokenSource = new CancellationTokenSource();

        ShutdownTask = ShutdownAsync(
            clientRegistry,
            mins,
            serverCancellationTokenSource,
            CancellationTokenSource.Token);
    }

    private static void SendMessage(IEnumerable<IChaosWorldClient> clients, string message)
    {
        foreach (var client in clients)
            client.SendServerMessage(ServerMessageType.ActiveMessage, message);
    }

    private static async Task ShutdownAsync(
        IClientRegistry<IChaosWorldClient> clients,
        int mins,
        CancellationTokenSource serverCancellationTokenSource,
        CancellationToken cancellationToken)
    {
        var deltaTime = new DeltaTime();
        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(250));

        var messageTimer = new PeriodicMessageTimer(
            TimeSpan.FromMinutes(mins),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(15),
            SHUTDOWN_FORMAT,
            message => SendMessage(clients, message));

        SendMessage(clients, StringExtensions.Inject(SHUTDOWN_FORMAT, "min".ToQuantity(mins)));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await periodicTimer.WaitForNextTickAsync(cancellationToken)
                                   .ConfigureAwait(false);
            } catch
            {
                break;
            }

            var delta = deltaTime.GetDelta;
            messageTimer.Update(delta);

            if (messageTimer.IntervalElapsed)
            {
                await serverCancellationTokenSource.CancelAsync();

                return;
            }
        }

        SendMessage(clients, "Server shutdown has been canceled");

        CancellationTokenSource = null;
        ShutdownTask = null;
    }
}