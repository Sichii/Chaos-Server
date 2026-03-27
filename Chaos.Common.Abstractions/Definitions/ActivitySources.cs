#region
using System.Diagnostics;
#endregion

namespace Chaos.Common.Abstractions.Definitions;

/// <summary>
///     Provides a centralized, extensible registry of ActivitySources
/// </summary>
public static class ActivitySources
{
    /// <summary>
    ///     The name of the packet processing activity source.
    /// </summary>
    public const string GENERAL_SOURCE_NAME = "chaos-server.general";

    public const string INTERNAL_SOURCE_NAME = "chaos-server.internal";

    /// <summary>
    ///     The ActivitySource for general traces.
    /// </summary>
    public static readonly ActivitySource GeneralActivitySource = new(GENERAL_SOURCE_NAME);

    /// <summary>
    ///     The ActivitySource for internal traces.
    /// </summary>
    public static readonly ActivitySource InternalActivitySource = new(INTERNAL_SOURCE_NAME);

    /// <summary>
    ///     Starts a new general activity with the given name.
    /// </summary>
    /// <param name="name">
    ///     The name of the activity/span.
    /// </param>
    /// <param name="kind">
    ///     The kind of activity (default is Server).
    /// </param>
    /// <returns>
    ///     The started Activity, or null if no listener is registered.
    /// </returns>
    public static Activity? StartGeneralActivity(string name, ActivityKind kind = ActivityKind.Server)
        => GeneralActivitySource.StartActivity(name, kind);

    /// <summary>
    ///     Starts a new internal activity with the given name. Only creates an activity if there is an ambient trace listener.
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
    public static Activity? StartInternalActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        // if no ambient trace do not create
        if (Activity.Current is null)
            return null;

        return InternalActivitySource.StartActivity(name, kind);
    }
}