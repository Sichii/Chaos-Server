#region
using System.Diagnostics;
using Chaos.Common.Abstractions.Definitions;
#endregion

namespace Chaos.Definitions;

/// <summary>
///     Provides ActivitySources for OpenTelemetry tracing throughout the Chaos server.
/// </summary>
public static class ChaosActivitySources
{
    /// <summary>
    ///     The name of the update loop activity source.
    /// </summary>
    public const string UPDATE_SOURCE_NAME = "chaos-server.updates";

    /// <summary>
    ///     The name of the world script execution activity source.
    /// </summary>
    public const string WORLD_SCRIPT_SOURCE_NAME = "chaos-server.worldscripts";

    /// <summary>
    ///     The ActivitySource for update loop traces (map updates, entity updates, etc.).
    /// </summary>
    public static readonly ActivitySource Updates = new(UPDATE_SOURCE_NAME);

    /// <summary>
    ///     The ActivitySource for world script execution traces.
    /// </summary>
    public static readonly ActivitySource WorldScripts = new(WORLD_SCRIPT_SOURCE_NAME);

    extension(ActivitySources)
    {
        /// <summary>
        ///     Starts a new update loop activity with the given name.
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
        public static Activity? StartUpdateActivity(string name, ActivityKind kind = ActivityKind.Server)
            => Updates.StartActivity(name, kind);

        /// <summary>
        ///     Starts a new world script execution activity with the given name.
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
        public static Activity? StartWorldScriptActivity(string name, ActivityKind kind = ActivityKind.Server)
            => WorldScripts.StartActivity(name, kind);
    }
}