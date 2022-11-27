namespace Chaos.CommandInterceptor;

public sealed class CommandHandlerConfiguration<T>
{
    public required Func<T, bool> AdminPredicate { get; set; }
    public required string Prefix { get; set; }
}