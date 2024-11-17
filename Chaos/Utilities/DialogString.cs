#region
using Chaos.Extensions.Common;
using JetBrains.Annotations;
#endregion

namespace Chaos.Utilities;

public static class DialogString
{
    public static string No => "No";
    public static string Ok => "Ok";
    public static string UnknownInput => "Huh...? I'm not sure what you mean...";
    public static string Yes => "Yes";
    public static string Format([StructuredMessageTemplate] string str, params ReadOnlySpan<object> objs) => str.Inject(objs);
    public static Lazy<string> From(Func<string> stringExpression) => new(stringExpression);
}