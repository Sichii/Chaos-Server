using Chaos.Networking.Options;

namespace Chaos.Servers.Options;

public record WorldOptions : ServerOptions, IWorldOptions
{
    public int AislingAssailIntervalMs { get; init; }
    public required int DropRange { get; init; }
    public static IWorldOptions Instance { get; set; } = null!;
    public required RedirectInfo LoginRedirect { get; init; }
    public required int MaxActionsPerSecond { get; init; }
    /// <inheritdoc />
    public int MaxChantTimeBurdenMs { get; init; }
    public required int MaxGoldHeld { get; init; }
    public required int MaximumAislingAc { get; init; }
    public required int MaximumMonsterAc { get; init; }
    public required int MaxLevel { get; init; }
    public required int MinimumAislingAc { get; init; }
    public required int MinimumMonsterAc { get; init; }
    public required int PickupRange { get; init; }
    /// <inheritdoc />
    public required bool ProhibitF5Walk { get; init; }
    /// <inheritdoc />
    public required bool ProhibitItemSwitchWalk { get; init; }
    /// <inheritdoc />
    public required bool ProhibitSpeedWalk { get; init; }
    public required int RefreshIntervalMs { get; init; }
    public required int SaveIntervalMins { get; init; }
    public required int TradeRange { get; init; }
    public required int UpdatesPerSecond { get; init; }

    public static void PostConfigure(WorldOptions options)
    {
        Instance = options;
        options.LoginRedirect.PopulateAddress();
    }
}