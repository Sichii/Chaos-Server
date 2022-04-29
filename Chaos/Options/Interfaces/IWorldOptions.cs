namespace Chaos.Options.Interfaces;

public interface IWorldOptions
{
    int RefreshIntervalMs { get; }
    int TradeRange { get; }
    int DropRange { get; }
    int PickupRange { get; }
}