using System.Threading.Tasks;
using Chaos.Clients.Interfaces;

namespace Chaos.Services.Serialization.Interfaces;

public interface ISaveManager<T>
{
    Task<T> LoadAsync(IWorldClient worldClient, string key);
    Task SaveAsync(T user);
}