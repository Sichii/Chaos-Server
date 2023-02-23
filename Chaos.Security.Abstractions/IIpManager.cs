using System.Net;

namespace Chaos.Security.Abstractions;

public interface IIpManager
{
    Task<bool> ShouldAllowAsync(IPAddress ipAddress);
}