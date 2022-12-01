using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class MerchantTemplateCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringMerchantTemplateCacheOptions : ExpiringFileCacheOptionsBase { }