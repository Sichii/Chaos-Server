using Chaos.Extensions.Common;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

/// <summary>
///     Represents an object that exists within the spell panel.
/// </summary>
public sealed class Spell : PanelObjectBase, IScripted<ISpellScript>
{
    public byte CastLines { get; set; }
    public ISpellScript Script { get; }
    public override SpellTemplate Template { get; }

    public Spell(
        SpellTemplate template,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null,
        int? elapsedMs = null
    )
        : base(template, uniqueId, elapsedMs)
    {
        Template = template;
        CastLines = template.CastLines;

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = scriptProvider.CreateScript<ISpellScript, Spell>(ScriptKeys, this);
    }

    /// <inheritdoc />
    public override string ToString() => $"{{ UId: {UniqueId}, Name: \"{Template.Name}\" }}";

    public void Use(SpellContext context)
    {
        if (!Script.CanUse(context))
            return;

        Script.OnUse(context);
        BeginCooldown(context.Source);
    }
}