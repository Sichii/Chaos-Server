using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class WorldMapNodeCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringWorldMapNodeCacheOptions : ExpiringFileCacheOptionsBase { }