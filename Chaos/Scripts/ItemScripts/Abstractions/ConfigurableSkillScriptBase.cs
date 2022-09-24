using System.Reflection;
using Chaos.Objects.Panel;

namespace Chaos.Scripts.ItemScripts.Abstractions;

public abstract class ConfigurableItemScriptBase : ItemScriptBase
{
    /// <inheritdoc />
    protected ConfigurableItemScriptBase(Item subject)
        : base(subject)
    {
        if (!subject.Template.ScriptVars.TryGetValue(ScriptKey, out var scriptVars))
            throw new InvalidOperationException(
                $"Item \"{subject.Template.Name}\" does not have script variables for script \"{ScriptKey}\"");

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