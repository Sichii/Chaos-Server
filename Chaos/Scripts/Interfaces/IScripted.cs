using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.Interfaces;

public interface IScripted { }

public interface IScripted<out TScript> : IScripted where TScript: IScript
{
    TScript? Script { get; }
}