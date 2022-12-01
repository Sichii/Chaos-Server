using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class WorldMapCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringWorldMapCacheOptions : ExpiringFileCacheOptionsBase { }