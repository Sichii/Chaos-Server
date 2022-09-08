namespace Chaos.Scripts.Abstractions;

public interface IScriptedMap : IScripted
{
    IMapScript Script { get; }
}