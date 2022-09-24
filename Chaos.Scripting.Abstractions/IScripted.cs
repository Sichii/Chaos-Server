namespace Chaos.Scripting.Abstractions;

public interface IScripted
{
    ISet<string> ScriptKeys { get; }
}