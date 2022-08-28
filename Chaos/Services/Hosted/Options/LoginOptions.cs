using Chaos.Entities.Networking;
using Chaos.Networking.Options;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Hosted.Options;

public record LoginOptions : ServerOptions
{
    public string NoticeMessage { get; set; } = null!;
    public ReservedRedirectInfo[] ReservedRedirects { get; set; } = Array.Empty<ReservedRedirectInfo>();
    public string StartingMapInstanceId { get; set; } = null!;
    public Point StartingPoint { get; set; }
    public string StartingPointStr { get; set; } = null!;
    public RedirectInfo WorldRedirect { get; set; } = null!;

    public static void PostConfigure(LoginOptions options, ILogger<LoginOptions> logger)
    {
        options.WorldRedirect.PopulateAddress();

        if (Point.TryParse(options.StartingPointStr, out var point))
            options.StartingPoint = point;
        else
            logger.LogError("Unable to parse starting point from config ({StartingPointStr})", options.StartingPointStr);
    }
}