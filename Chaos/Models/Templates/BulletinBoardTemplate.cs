using Chaos.Common.Abstractions;

namespace Chaos.Models.Templates;

public sealed class BulletinBoardTemplate
{
    public required ushort Id { get; init; }
    public required string Name { get; init; }
    public required ICollection<string> ScriptKeys { get; init; }
    public required IDictionary<string, IScriptVars> ScriptVars { get; init; } =
        new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase);
    public required string TemplateKey { get; init; }
}