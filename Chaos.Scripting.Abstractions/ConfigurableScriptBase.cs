using System.Reflection;
using Chaos.Common.Abstractions;

namespace Chaos.Scripting.Abstractions;

/// <summary>
///     Defines the basic functionality of a script that can have variables loaded into it
/// </summary>
/// <remarks>
///     This kind of script accepts variables through it's <see cref="Chaos.Common.Abstractions.IScriptVars" /> parameter. It then scans the
///     inheritance
///     chain and populates all non-public, writable, instanced properties automatically with values from the
///     <see cref="Chaos.Common.Abstractions.IScriptVars" />
/// </remarks>
/// <typeparam name="T">The <see cref="Chaos.Scripting.Abstractions.IScripted" /> object this script is attached to</typeparam>
public abstract class ConfigurableScriptBase<T> : SubjectiveScriptBase<T> where T: IScripted
{
    /// <summary>
    ///     The variables that will be loaded into the script
    /// </summary>
    protected IScriptVars ScriptVars { get; }

    protected ConfigurableScriptBase(T subject, IScriptVars scriptVars)
        : base(subject)
    {
        ScriptVars = scriptVars;

        PopulateVars();
    }

    protected ConfigurableScriptBase(T subject, Func<string, IScriptVars> scriptVarsFactory)
        : base(subject)
    {
        try
        {
            ScriptVars = scriptVarsFactory(ScriptKey);
        } catch
        {
            //ignored
        }

        if (ScriptVars == null)
            throw new InvalidOperationException($"ScriptVars for script \"{GetType().FullName}\" were not found, and are required");

        PopulateVars();
    }

    /// <summary>
    ///     Populates the script's properties with values from <see cref="ScriptVars" />
    /// </summary>
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