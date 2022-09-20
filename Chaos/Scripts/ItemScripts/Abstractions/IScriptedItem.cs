using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts.Abstractions;

public interface IScriptedItem : IScripted
{
    IItemScript Script { get; }
}