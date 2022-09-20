using Chaos.Objects.World;

namespace Chaos.Commands.Abstractions;

public interface ICommandInterceptor
{
    void HandleCommand(Aisling aisling, string commandStr);
    bool IsCommand(string commandStr);
}