#region
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Services.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;
#endregion

namespace Chaos.Services.Storage;

public class AislingFacadeCache
{
    private readonly IFacadeStore<Aisling> AislingStore;
    private readonly IMemoryCache Cache;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly string KeyPrefix;
    private readonly ILogger Logger;
    private readonly Lock Sync = new();

    public AislingFacadeCache(
        IMemoryCache cache,
        ILogger<AislingFacadeCache> logger,
        IFacadeStore<Aisling> aislingStore,
        IClientRegistry<IChaosWorldClient> clientRegistry)
    {
        Cache = cache;
        AislingStore = aislingStore;
        ClientRegistry = clientRegistry;
        Logger = logger;
        KeyPrefix = $"{nameof(Aisling)}Facade___".ToLowerInvariant();
    }

    /// <summary>
    ///     Constructs a cache key for the specified object type.
    /// </summary>
    /// <param name="key">
    ///     The key to be used for the cache entry.
    /// </param>
    /// <returns>
    ///     A constructed cache key.
    /// </returns>
    protected virtual string ConstructKeyForType(string key) => KeyPrefix + key.ToLowerInvariant();

    /// <summary>
    ///     Deconstructs a cache key for the specified object type.
    /// </summary>
    /// <param name="key">
    ///     The key to deconstruct
    /// </param>
    protected virtual string DeconstructKeyForType(string key) => key.Replace(KeyPrefix, string.Empty, StringComparison.OrdinalIgnoreCase);

    public Aisling? Get(string name)
    {
        //if there's a client logged on with the name, return that
        var loggedOnClient = ClientRegistry.FirstOrDefault(client => client.Aisling.Name.EqualsI(name));

        if (loggedOnClient is not null)
            return loggedOnClient.Aisling;

        using var @lock = Sync.EnterScope();

        //otherwise, load and cache the aisling
        var key = ConstructKeyForType(name);

        if (Cache.TryGetValue(key, out Aisling? aisling))
            return aisling;

        using var entry = Cache.CreateEntry(key);
        entry.SetSlidingExpiration(TimeSpan.FromHours(1));

        try
        {
            aisling = AislingStore.Load(name);
            entry.Value = aisling;

            return aisling;
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Load)
                  .LogWarning(e, "Failed to load aisling {@AislingName}", name);

            return null;
        }
    }
}