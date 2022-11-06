namespace Chaos.Utilities;

public static class DialogString
{
    public static Lazy<string> No { get; } = new("No");
    public static Lazy<string> Ok { get; } = new("Ok");
    public static Lazy<string> UnknownInput { get; } = new("Huh...? I'm not sure what you mean...");
    public static Lazy<string> Yes { get; } = new("Yes");
    public static Lazy<string> Constant(string constantString) => new(constantString);
    public static Lazy<string> From(Func<string> stringExpression) => new(stringExpression);
}