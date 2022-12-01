using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class LootTableCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringLootTableCacheOptions : ExpiringFileCacheOptionsBase { }