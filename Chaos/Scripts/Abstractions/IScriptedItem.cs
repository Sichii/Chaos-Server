namespace Chaos.Scripts.Abstractions;

public interface IScriptedItem : IScripted
{
    IItemScript Script { get; }
}