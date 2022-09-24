namespace Chaos.Scripting.Abstractions;

public abstract class ScriptBase : IScript, IEquatable<ScriptBase>
{
    public string ScriptKey { get; }

    protected ScriptBase() => ScriptKey = GetScriptKey(GetType());

    public bool Equals(ScriptBase? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return string.Equals(ScriptKey, other.ScriptKey, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is ScriptBase other && Equals(other));

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(ScriptKey);
    public static string GetScriptKey(Type type) => type.Name.Replace("Script", string.Empty, StringComparison.OrdinalIgnoreCase);
}