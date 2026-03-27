#region
using System.Diagnostics;
using Chaos.Common.Abstractions.Definitions;
#endregion

namespace Chaos.Networking.Abstractions.Definitions;

/// <summary>
///     Provides an ActivitySource for packet processing traces.
/// </summary>
public static class NetworkingActivitySources
{
    /// <summary>
    ///     The name of the packet processing activity source.
    /// </summary>
    public const string PACKET_SOURCE_NAME = "chaos-server.packets";

    /// <summary>
    ///     The ActivitySource for packet processing traces.
    /// </summary>
    public static readonly ActivitySource PacketActivitySource = new(PACKET_SOURCE_NAME);

    extension(ActivitySources)
    {
        /// <summary>
        ///     Starts a new packet processing activity with the given name.
        /// </summary>
        /// <param name="name">
        ///     The name of the activity/span.
        /// </param>
        /// <param name="kind">
        ///     The kind of activity (default is Internal).
        /// </param>
        /// <returns>
        ///     The started Activity, or null if no listener is registered.
        /// </returns>
        public static Activity? StartPacketActivity(string name, ActivityKind kind = ActivityKind.Server)
            => PacketActivitySource.StartActivity(name, kind);
    }
}