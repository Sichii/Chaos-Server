using System;
using Chaos.Core.Geometry;
using Chaos.Networking.Options;

namespace Chaos.Options;

public record LoginOptions : ServerOptions
{
    public string NoticeMessage { get; set; } = null!;
    public ReservedRedirectInfo[] ReservedRedirects { get; set; } = Array.Empty<ReservedRedirectInfo>();
    public string StartingMapInstanceId { get; set; } = null!;
    public Point StartingPoint { get; set; }
    public RedirectInfo WorldRedirect { get; set; } = null!;

    public static void PostConfigure(LoginOptions options) => options.WorldRedirect.PopulateAddress();
}