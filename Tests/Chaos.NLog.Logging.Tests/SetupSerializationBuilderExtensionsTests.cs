#region
using Chaos.NLog.Logging.Abstractions;
using Chaos.NLog.Logging.Extensions;
using FluentAssertions;
using Moq;
using NLog.Config;
#endregion

namespace Chaos.NLog.Logging.Tests;

[NotInParallel]
public sealed class SetupSerializationBuilderExtensionsTests
{
    [Test]
    public void RegisterCollectionTransformations_MultipleTransformations_ShouldWork()
    {
        // Arrange
        var mockBuilder = new Mock<ISetupSerializationBuilder>();

        var collection1 = new MultiTransformCollection1
        {
            Value = "one"
        };

        var collection2 = new MultiTransformCollection2
        {
            Value = "two"
        };

        // Act
        mockBuilder.Object.RegisterCollectionTransformations(
            (Func<MultiTransformCollection1, object>)(c => new
            {
                Result = c.Value + "-transformed1"
            }),
            (Func<MultiTransformCollection2, object>)(c => new
            {
                Result = c.Value + "-transformed2"
            }));

        var result1 = SetupSerializationBuilderExtensions.Transform(collection1);
        var result2 = SetupSerializationBuilderExtensions.Transform(collection2);

        // Assert
        result1.GetType()
               .GetProperty("Result")!.GetValue(result1)
               .Should()
               .Be("one-transformed1");

        result2.GetType()
               .GetProperty("Result")!.GetValue(result2)
               .Should()
               .Be("two-transformed2");
    }

    [Test]
    public void RegisterCollectionTransformations_NonTransformableParameter_ShouldThrow()
    {
        // Arrange
        var mockBuilder = new Mock<ISetupSerializationBuilder>();

        // Act — pass a delegate with non-ITransformableCollection parameter
        Func<string, object> badDelegate = s => new
        {
            Value = s
        };
        var act = () => mockBuilder.Object.RegisterCollectionTransformations(badDelegate);

        // Assert
        act.Should()
           .Throw<ArgumentException>()
           .WithMessage("*assignable*");
    }

    [Test]
    public void RegisterCollectionTransformations_NullTransformation_ShouldThrow()
    {
        // Arrange
        var mockBuilder = new Mock<ISetupSerializationBuilder>();

        // Act — pass an array containing a null element to hit the null check inside the loop
        var act = () => mockBuilder.Object.RegisterCollectionTransformations(
            new Delegate[]
            {
                null!
            });

        // Assert
        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void RegisterCollectionTransformations_StaticMethod_ShouldWork()
    {
        // Arrange
        var mockBuilder = new Mock<ISetupSerializationBuilder>();

        var testCollection = new StaticTransformTestCollection
        {
            Data = "static"
        };

        // Act — register a static method (target == null)
        mockBuilder.Object.RegisterCollectionTransformations((Func<StaticTransformTestCollection, object>)StaticTransform);
        var result = SetupSerializationBuilderExtensions.Transform(testCollection);

        // Assert
        result.Should()
              .NotBeNull();
    }

    [Test]
    public void RegisterCollectionTransformations_WhenRegisteredAndTransformed_ShouldWork()
    {
        // Arrange
        var mockBuilder = new Mock<ISetupSerializationBuilder>();

        var testCollection = new IsolatedTestTransformableCollection
        {
            Value = "test"
        };

        // Act - Register and immediately test transformation
        mockBuilder.Object.RegisterCollectionTransformations(TestTransform);
        var result = SetupSerializationBuilderExtensions.Transform(testCollection);

        // Assert
        result.Should()
              .NotBeNull();
        var resultType = result.GetType();

        var transformedValueProperty = resultType.GetProperty("TransformedValue");
        var typeProperty = resultType.GetProperty("Type");

        transformedValueProperty.Should()
                                .NotBeNull();

        typeProperty.Should()
                    .NotBeNull();

        transformedValueProperty.GetValue(result)
                                .Should()
                                .Be("TEST");

        typeProperty.GetValue(result)
                    .Should()
                    .Be("transformed");

        return;

        static object TestTransform(IsolatedTestTransformableCollection collection)
            => new
            {
                TransformedValue = collection.Value.ToUpper(),
                Type = "transformed"
            };
    }

    [Test]
    public void RegisterCollectionTransformations_WrongParameterCount_ShouldThrow()
    {
        // Arrange
        var mockBuilder = new Mock<ISetupSerializationBuilder>();

        // Act — pass a delegate with 2 parameters
        Func<ITransformableCollection, int, object> badDelegate = (_, _) => new object();
        var act = () => mockBuilder.Object.RegisterCollectionTransformations(badDelegate);

        // Assert
        act.Should()
           .Throw<ArgumentException>()
           .WithMessage("*exactly one parameter*");
    }

    private static object StaticTransform(StaticTransformTestCollection c)
        => new
        {
            StaticResult = c.Data.ToUpper()
        };

    [Test]
    public void Transform_UnregisteredType_ShouldThrow()
    {
        // Arrange
        var unregistered = new UnregisteredTransformableCollection();

        // Act
        var act = () => SetupSerializationBuilderExtensions.Transform(unregistered);

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("*No transformation registered*");
    }

    private sealed class IsolatedTestTransformableCollection : ITransformableCollection
    {
        public string Value { get; init; } = "";
    }

    private sealed class MultiTransformCollection1 : ITransformableCollection
    {
        public string Value { get; init; } = "";
    }

    private sealed class MultiTransformCollection2 : ITransformableCollection
    {
        public string Value { get; init; } = "";
    }

    private sealed class StaticTransformTestCollection : ITransformableCollection
    {
        public string Data { get; init; } = "";
    }

    private sealed class UnregisteredTransformableCollection : ITransformableCollection;
}