using Chaos.Containers;

namespace Chaos.Services.Storage.Abstractions;

public interface IShardGenerator
{
    public MapInstance CreateShardOfInstance(string instanceId);
}