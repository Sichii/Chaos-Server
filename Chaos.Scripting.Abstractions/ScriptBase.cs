using Chaos.Extensions.Common;

namespace Chaos.Scripting.Abstractions;

/// <inheritdoc cref="Chaos.Scripting.Abstractions.IScript" />
public abstract class ScriptBase : IScript, IEquatable<ScriptBase>
{
    /// <inheritdoc />
    public string ScriptKey { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScriptBase" /> class.
    /// </summary>
    protected ScriptBase() => ScriptKey = GetScriptKey(GetType());

    /// <inheritdoc />
    public bool Equals(ScriptBase? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return string.Equals(ScriptKey, other.ScriptKey, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is ScriptBase other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(ScriptKey);

    /// <summary>
    ///     Generates a script key from a script type
    /// </summary>
    /// <param name="type">
    ///     A type object
    /// </param>
    /// <returns>
    ///     The name of the type without "Script" in it
    /// </returns>
    public static string GetScriptKey(Type type) => type.Name.ReplaceI("Script", string.Empty);
}