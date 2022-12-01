using Chaos.Networking.Options;

namespace Chaos.Services.Servers.Options;

public sealed record WorldOptions : ServerOptions, IWorldOptions
{
    /// <inheritdoc />
    public required int AislingAssailIntervalMs { get; init; }
    /// <inheritdoc />
    public required int DropRange { get; init; }
    /// <inheritdoc />
    public override string HostName { get; set; } = string.Empty;
    public static IWorldOptions Instance { get; set; } = null!;
    public required RedirectInfo LoginRedirect { get; init; }
    /// <inheritdoc />
    public required int MaxActionsPerSecond { get; init; }
    /// <inheritdoc />
    public required int MaxChantTimeBurdenMs { get; init; }
    /// <inheritdoc />
    public required int MaxGoldHeld { get; init; }
    /// <inheritdoc />
    public required int MaxGroupSize { get; init; }
    /// <inheritdoc />
    public required int MaximumAislingAc { get; init; }
    /// <inheritdoc />
    public required int MaximumMonsterAc { get; init; }
    /// <inheritdoc />
    public required int MaxLevel { get; init; }
    /// <inheritdoc />
    public required int MinimumAislingAc { get; init; }
    /// <inheritdoc />
    public required int MinimumMonsterAc { get; init; }
    /// <inheritdoc />
    public required int PickupRange { get; init; }
    /// <inheritdoc />
    public required bool ProhibitF5Walk { get; init; }
    /// <inheritdoc />
    public required bool ProhibitItemSwitchWalk { get; init; }
    /// <inheritdoc />
    public required bool ProhibitSpeedWalk { get; init; }
    /// <inheritdoc />
    public required int RefreshIntervalMs { get; init; }
    /// <inheritdoc />
    public required double SaveIntervalMins { get; init; }
    /// <inheritdoc />
    public required int TradeRange { get; init; }
    /// <inheritdoc />
    public required int UpdatesPerSecond { get; init; }

    public static void PostConfigure(WorldOptions options)
    {
        Instance = options;
        options.LoginRedirect.PopulateAddress();
    }
}