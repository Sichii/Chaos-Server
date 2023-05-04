using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Models.Panel;

/// <summary>
///     Represents an object that exists within the spell panel.
/// </summary>
public sealed class Spell : PanelEntityBase, IScripted<ISpellScript>
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
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Script.Update(delta);
    }

    public void Use(SpellContext context)
    {
        Script.OnUse(context);
        BeginCooldown(context.Source);
    }
}