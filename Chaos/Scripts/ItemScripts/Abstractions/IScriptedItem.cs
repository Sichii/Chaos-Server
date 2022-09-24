using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.ItemScripts.Abstractions;

public interface IScriptedItem : IScripted
{
    IItemScript Script { get; }
}