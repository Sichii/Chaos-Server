namespace Chaos.Services.Hosted.Options;

public interface IWorldOptions
{
    int DropRange { get; }
    int PickupRange { get; }
    int RefreshIntervalMs { get; }
    int TradeRange { get; }
    int MaxGoldHeld { get; }
}