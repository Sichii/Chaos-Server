using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class DictionaryExtensionsTests
{
    [Fact]
    public void TryRemove_EmptyDictionary_ReturnsFalseAndValueIsDefault()
    {
        var dictionary = new Dictionary<int, string>();

        var result = dictionary.TryRemove(2, out var removedValue);

        result.Should()
              .BeFalse("because the dictionary is empty");

        removedValue.Should()
                    .BeNull("because there's no associated value in an empty dictionary");
    }

    [Fact]
    public void TryRemove_KeyDoesNotExistInDictionary_ReturnsFalseAndValueIsDefault()
    {
        var dictionary = new Dictionary<int, string>
        {
            {
                1, "one"
            },
            {
                3, "three"
            }
        };

        var result = dictionary.TryRemove(2, out var removedValue);

        result.Should()
              .BeFalse("because the key doesn't exist in the dictionary");

        removedValue.Should()
                    .BeNull("because there's no associated value for the non-existent key");
    }

    [Fact]
    public void TryRemove_KeyExistsInDictionary_ReturnsTrueAndRemovesKeyValue()
    {
        var dictionary = new Dictionary<int, string>
        {
            {
                1, "one"
            },
            {
                2, "two"
            },
            {
                3, "three"
            }
        };

        var result = dictionary.TryRemove(2, out var removedValue);

        result.Should()
              .BeTrue("because the key exists in the dictionary");

        removedValue.Should()
                    .Be("two", "because that's the associated value of key 2");

        dictionary.ContainsKey(2)
                  .Should()
                  .BeFalse("because the key-value pair should have been removed");
    }
}