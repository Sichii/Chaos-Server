namespace Chaos.CommandInterceptor.Abstractions;

public interface ICommandInterceptor<in T>
{
    void HandleCommand(T obj, string commandStr);
    bool IsCommand(string commandStr);
}