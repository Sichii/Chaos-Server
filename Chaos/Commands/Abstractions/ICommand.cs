using Chaos.Objects.World;

namespace Chaos.Commands.Abstractions;

public interface ICommand
{
    void Execute(Aisling aisling, params string[] args);
}