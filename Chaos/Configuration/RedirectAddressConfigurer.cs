using System.Net;
using System.Net.Sockets;
using Chaos.Networking.Abstractions;
using Microsoft.Extensions.Options;

namespace Chaos.Configuration;

public abstract class RedirectAddressConfigurer : IPostConfigureOptions<IConnectionInfo>
{
    /// <inheritdoc />
    public virtual void PostConfigure(string? name, IConnectionInfo options) =>
        options.Address = Dns.GetHostAddresses(options.HostName).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;
}