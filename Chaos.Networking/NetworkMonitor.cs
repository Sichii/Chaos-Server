#region
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Microsoft.Extensions.Logging;
using TDigestNet;
#endregion

namespace Chaos.Networking;

/// <summary>
///     Stores the execution time of network packets. This is used to monitor the performance of the network layer.
/// </summary>
public sealed class NetworkMonitor
{
    private readonly SocketClientBase Client;
    private readonly ConcurrentDictionary<byte, TDigest> Digests;
    private readonly ILogger Logger;
    private Task MonitorLoopTask;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NetworkMonitor" /> class
    /// </summary>
    public NetworkMonitor(SocketClientBase client, ILogger logger)
    {
        Client = client;
        Logger = logger;
        Digests = new ConcurrentDictionary<byte, TDigest>();

        MonitorLoopTask = RunMonitorLoop();
    }

    /// <summary>
    ///     Digests the execution time of a network packet.
    /// </summary>
    public void Digest(byte opcode, TimeSpan executionTime)
        => Digests.AddOrUpdate(
            opcode,
            _ =>
            {
                var newDigest = new TDigest();
                newDigest.Add(executionTime.Ticks);

                return newDigest;
            },
            (_, digest) =>
            {
                digest.Add(executionTime.Ticks);

                return digest;
            });

    private void PrintStatistics()
    {
        var digests = Digests.ToArray();

        var serverTopic = Client switch
        {
            ILobbyClient => Topics.Servers.LobbyServer,
            ILoginClient => Topics.Servers.LoginServer,
            _            => Topics.Servers.WorldServer
        };

        var logEvent = Logger.WithTopics(serverTopic, Topics.Entities.Client, Topics.Entities.NetworkMonitor)
                             .WithProperty(Client);

        foreach ((var opcode, var digest) in digests.OrderBy(d => d.Key))
        {
            var average = digest.Average / TimeSpan.TicksPerMillisecond;
            var max = digest.Max / TimeSpan.TicksPerMillisecond;
            var count = digest.Count;
            var upperPct = digest.Quantile(0.95) / TimeSpan.TicksPerMillisecond;
            var median = digest.Quantile(0.5) / TimeSpan.TicksPerMillisecond;

            logEvent = logEvent.WithProperty(
                $"Average: {average:N1}ms, Median: {median:N1}ms, 95th%: {upperPct:N1}ms, Max: {max:N1}ms, Samples: {count}",
                $"OpCode[{opcode:D3}]");
        }

        Digests.Clear();

        logEvent.LogTrace("Network Monitor [{@ClientId}]", Client.Id);
    }

    private Task RunMonitorLoop()
        => Task.Run(async () =>
        {
            var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (Client.Connected)
                try
                {
                    await periodicTimer.WaitForNextTickAsync();

                    PrintStatistics();
                } catch
                {
                    //ignored
                }
        });
}