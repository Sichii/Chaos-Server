using System.Reflection;
using Chaos.Common.Collections;
using Chaos.Objects.Panel;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public abstract class ConfigurableSkillScriptBase : SkillScriptBase
{
    /// <inheritdoc />
    protected ConfigurableSkillScriptBase(Skill subject)
        : base(subject)
    {
        if (!subject.Template.ScriptVars.TryGetValue(ScriptKey, out var scriptVars))
            throw new InvalidOperationException(
                $"Skill \"{subject.Template.Name}\" does not have script variables for script \"{ScriptKey}\"");

        var props = GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(
                        prop => prop.CanWrite
                                && (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType)
                                .IsAssignableTo(typeof(IConvertible)));

        foreach (var prop in props)
        {
            var type = prop.PropertyType;
            var value = scriptVars.Get(type, prop.Name);

            if (value != null)
                prop.SetValue(this, value);
        }
    }
}