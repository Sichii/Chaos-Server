using Chaos.Storage.Abstractions;

namespace Chaos.Storage.Options;

public sealed class SkillTemplateCacheOptions : SimpleFileCacheOptionsBase { }

public sealed class ExpiringSkillTemplateCacheOptions : ExpiringFileCacheOptionsBase { }