using Chaos.Core.Collections;
using Chaos.Objects.Panel;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public abstract class ConfigurableSpellScriptBase : SpellScriptBase
{
    protected DynamicVars ScriptVars { get; }
    
    /// <inheritdoc />
    protected ConfigurableSpellScriptBase(Spell subject)
        : base(subject)
    {
        if (!subject.Template.ScriptVars.TryGetValue(ScriptKey, out var scriptVars))
            throw new InvalidOperationException(
                $"Spell \"{subject.Template.Name}\" does not have script variables for script \"{ScriptKey}\"");

        ScriptVars = scriptVars;
    }
}