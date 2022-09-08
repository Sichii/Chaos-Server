namespace Chaos.Scripts.Abstractions;

public interface IScripted
{
    ISet<string> ScriptKeys { get; }
}