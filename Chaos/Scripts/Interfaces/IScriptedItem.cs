namespace Chaos.Scripts.Interfaces;

public interface IScriptedItem : IScripted
{
    IItemScript Script { get; }
}