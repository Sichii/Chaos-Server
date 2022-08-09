namespace Chaos.Services.Hosted.Options;

public interface IWorldOptions
{
    int DropRange { get; }
    int MaxGoldHeld { get; }
    int MaximumAislingAc { get; }
    int MaximumMonsterAc { get; }
    int MinimumAislingAc { get; }
    int MinimumMonsterAc { get; }
    int PickupRange { get; }
    int RefreshIntervalMs { get; }
    int TradeRange { get; }
}