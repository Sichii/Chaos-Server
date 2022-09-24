namespace Chaos.CommandInterceptor.Abstractions;

public interface ICommand<in T>
{
    void Execute(T source, params string[] args);
}