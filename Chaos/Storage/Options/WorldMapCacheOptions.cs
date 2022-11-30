using Chaos.Storage.Abstractions;

namespace Chaos.Storage.Options;

public sealed class WorldMapCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringWorldMapCacheOptions : ExpiringFileCacheOptionsBase { }