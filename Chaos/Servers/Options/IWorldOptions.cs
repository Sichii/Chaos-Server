namespace Chaos.Servers.Options;

public interface IWorldOptions
{
    int AislingAssailIntervalMs { get; }
    int DropRange { get; }
    int MaxActionsPerSecond { get; }
    int MaxChantTimeBurdenMs { get; }
    int MaxGoldHeld { get; }
    int MaximumAislingAc { get; }
    int MaximumMonsterAc { get; }
    int MaxLevel { get; }
    int MinimumAislingAc { get; }
    int MinimumMonsterAc { get; }
    int PickupRange { get; }
    bool ProhibitF5Walk { get; }
    bool ProhibitItemSwitchWalk { get; }
    bool ProhibitSpeedWalk { get; }
    int RefreshIntervalMs { get; }
    double SaveIntervalMins { get; }
    int TradeRange { get; }
}