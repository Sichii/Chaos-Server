using System.Reflection;
using Chaos.Common.Collections;

namespace Chaos.Scripting.Abstractions;

public abstract class ConfigurableScriptBase<T> : SubjectiveScriptBase<T> where T: IScripted
{
    private readonly DynamicVars ScriptVars;

    /// <inheritdoc />
    protected ConfigurableScriptBase(T subject, DynamicVars scriptVars)
        : base(subject)
    {
        ScriptVars = scriptVars;

        PopulateVars();
    }

    protected ConfigurableScriptBase(T subject, Func<string, DynamicVars> scriptVarsFactory)
        : base(subject)
    {
        ScriptVars = scriptVarsFactory(ScriptKey);

        PopulateVars();
    }

    private void PopulateVars()
    {
        var props = GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(prop => prop.CanWrite);

        foreach (var prop in props)
        {
            var type = prop.PropertyType;
            var value = ScriptVars.Get(type, prop.Name);

            if (value != null)
                prop.SetValue(this, value);
        }
    }
}