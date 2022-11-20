using System.Net;
using System.Net.Sockets;
using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Options;

public record RedirectInfo : IRedirectInfo
{
    public IPAddress Address { get; set; } = null!;
    public virtual string HostName { get; set; } = null!;
    public int Port { get; set; }

    public void PopulateAddress() =>
        Address = Dns.GetHostAddresses(HostName).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;
}