using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class SpellTemplateCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringSpellTemplateCacheOptions : ExpiringFileCacheOptionsBase { }