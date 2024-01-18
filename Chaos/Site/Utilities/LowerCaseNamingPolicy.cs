using System.Text.Json;

namespace Chaos.Site.Utilities;

public sealed class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToLower();
}