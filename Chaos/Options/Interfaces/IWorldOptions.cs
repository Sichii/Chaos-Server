namespace Chaos.Options.Interfaces;

public interface IWorldOptions
{
    int DropRange { get; }
    int PickupRange { get; }
    int RefreshIntervalMs { get; }
    int TradeRange { get; }
}