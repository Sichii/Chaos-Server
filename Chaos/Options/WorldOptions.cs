using Chaos.Networking.Options;
using Chaos.Options.Interfaces;

namespace Chaos.Options;

public record WorldOptions : ServerOptions, IWorldOptions
{
    public RedirectInfo LoginRedirect { get; set; } = null!;
    public int RefreshIntervalMs { get; set; }
    public int TradeRange { get; set; }
    public int DropRange { get; set; }
    public int PickupRange { get; set; }

    public static void PostConfigure(WorldOptions options) => options.LoginRedirect.PopulateAddress();

}