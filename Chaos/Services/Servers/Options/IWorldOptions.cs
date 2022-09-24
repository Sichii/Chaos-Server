namespace Chaos.Services.Servers.Options;

public interface IWorldOptions
{
    int AislingAssailIntervalMs { get; }
    int DropRange { get; }
    int MaxActionsPerSecond { get; }
    int MaxGoldHeld { get; }
    int MaximumAislingAc { get; }
    int MaximumMonsterAc { get; }
    int MaxLevel { get; }
    int MinimumAislingAc { get; }
    int MinimumMonsterAc { get; }
    int PickupRange { get; }
    int RefreshIntervalMs { get; }
    int TradeRange { get; }
}