using Chaos.Objects.Panel.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

/// <summary>
///     Represents an object that exists within the spell panel.
/// </summary>
public class Spell : PanelObjectBase, IScriptedSpell
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

    public void Use(ActivationContext context) => Script.OnUse(context);
}