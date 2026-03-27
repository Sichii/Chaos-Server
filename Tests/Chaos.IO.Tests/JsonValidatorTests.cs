#region
using System.Text;
using System.Text.Json;
using Chaos.IO.Json;
using FluentAssertions;
#endregion

namespace Chaos.IO.Tests;

public sealed class JsonValidatorTests
{
    [Test]
    public void EnsureValidJson_EmptyStream_ThrowsJsonException()
    {
        using var stream = new MemoryStream([]);

        // ReSharper disable once AccessToDisposedClosure
        var act = () => JsonValidator.EnsureValidJson(stream);

        act.Should()
           .Throw<JsonException>();
    }

    [Test]
    public void EnsureValidJson_InvalidJson_ThrowsJsonException()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{invalid json}"));

        // ReSharper disable once AccessToDisposedClosure
        var act = () => JsonValidator.EnsureValidJson(stream);

        act.Should()
           .Throw<JsonException>();
    }

    [Test]
    public void EnsureValidJson_RestoresStreamPosition_AfterFailure()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{bad"));
        stream.Position = 0;

        try
        {
            JsonValidator.EnsureValidJson(stream);
        } catch (JsonException)
        {
            // expected
        }

        stream.Position
              .Should()
              .Be(0);
    }

    [Test]
    public void EnsureValidJson_RestoresStreamPosition_AfterSuccess()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("[1, 2, 3]"));
        stream.Position = 0;

        JsonValidator.EnsureValidJson(stream);

        stream.Position
              .Should()
              .Be(0);
    }

    [Test]
    public void EnsureValidJson_ValidJson_DoesNotThrow()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"key\": \"value\"}"));

        // ReSharper disable once AccessToDisposedClosure
        var act = () => JsonValidator.EnsureValidJson(stream);

        act.Should()
           .NotThrow();
    }
}