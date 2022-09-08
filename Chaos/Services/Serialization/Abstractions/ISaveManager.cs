using System.Threading.Tasks;
using Chaos.Clients.Abstractions;

namespace Chaos.Services.Serialization.Abstractions;

public interface ISaveManager<T>
{
    Task<T> LoadAsync(IWorldClient worldClient, string key);
    Task SaveAsync(T user);
}