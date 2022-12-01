using Chaos.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class DialogTemplateCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringDialogTemplateCacheOptions : ExpiringFileCacheOptionsBase { }