using Chaos.Core.Collections;
using Chaos.Objects.Panel;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public abstract class ConfigurableSkillScriptBase : SkillScriptBase
{
    protected DynamicVars ScriptVars { get; }
    
    /// <inheritdoc />
    protected ConfigurableSkillScriptBase(Skill subject)
        : base(subject)
    {
        if (!subject.Template.ScriptVars.TryGetValue(ScriptKey, out var scriptVars))
            throw new InvalidOperationException(
                $"Skill \"{subject.Template.Name}\" does not have script variables for script \"{ScriptKey}\"");

        ScriptVars = scriptVars;
    }
}