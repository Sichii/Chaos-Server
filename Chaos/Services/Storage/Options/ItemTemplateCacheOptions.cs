using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class ItemTemplateCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringItemTemplateCacheOptions : ExpiringFileCacheOptionsBase { }