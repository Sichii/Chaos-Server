namespace Chaos.CommandInterceptor;

public class CommandInterceptorConfiguration<T>
{
    public required Func<T, bool> AdminPredicate { get; set; }
    public required Func<T, string> IdentifierSelector { get; set; }
    public required string Prefix { get; set; }
}