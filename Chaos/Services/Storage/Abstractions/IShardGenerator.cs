using Chaos.Collections;

namespace Chaos.Services.Storage.Abstractions;

public interface IShardGenerator
{
    public MapInstance CreateShardOfInstance(string instanceId);
}