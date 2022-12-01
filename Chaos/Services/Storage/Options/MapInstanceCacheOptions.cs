using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class MapInstanceCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringMapInstanceCacheOptions : ExpiringFileCacheOptionsBase { }