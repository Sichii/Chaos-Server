using System.Net;
using System.Net.Sockets;

namespace Chaos.Entities.Networking;

public record RedirectInfo
{
    public IPAddress Address { get; set; } = null!;
    public string HostName { get; set; } = null!;
    public int Port { get; set; }

    public void PopulateAddress() =>
        Address = Dns.GetHostAddresses(HostName).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;
}