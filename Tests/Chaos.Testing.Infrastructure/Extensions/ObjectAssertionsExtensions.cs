using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Chaos.Testing.Infrastructure.Extensions;

public static class ObjectAssertionsExtensions
{
    public static void BeEquivalentToJson(this StringAssertions assertions, string expectedJson)
    {
        var subjectElement = JsonSerializer.Deserialize<JsonElement>(assertions.Subject);
        var expectedElement = JsonSerializer.Deserialize<JsonElement>(expectedJson);

        subjectElement.Should().BeEquivalentTo(expectedElement, obj => obj.ComparingByMembers<JsonElement>());
    }
}