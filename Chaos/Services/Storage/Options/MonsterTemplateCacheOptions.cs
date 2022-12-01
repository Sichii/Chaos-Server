using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class MonsterTemplateCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringMonsterTemplateCacheOptions : ExpiringFileCacheOptionsBase { }