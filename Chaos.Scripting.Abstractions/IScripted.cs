namespace Chaos.Scripting.Abstractions;

public interface IScripted
{
    ISet<string> ScriptKeys { get; }
}

public interface IScripted<out TScript> : IScripted where TScript: IScript
{
    TScript Script { get; }
}