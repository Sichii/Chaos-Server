using System.Net;

namespace Chaos.Security.Abstractions;

public interface IAccessManager
{
    Task<bool> ShouldAllowAsync(IPAddress ipAddress);
}