using Chaos.Extensions.Common;
using JetBrains.Annotations;

namespace Chaos.Utilities;

public static class DialogString
{
    public static Lazy<string> No { get; } = new("No");
    public static Lazy<string> Ok { get; } = new("Ok");
    public static Lazy<string> UnknownInput { get; } = new("Huh...? I'm not sure what you mean...");
    public static Lazy<string> Yes { get; } = new("Yes");
    public static Lazy<string> Constant(string constantString) => new(constantString);

    public static string Format([StructuredMessageTemplate] string str, params object[] objs) => str.Inject(objs);
    public static Lazy<string> From(Func<string> stringExpression) => new(stringExpression);
}