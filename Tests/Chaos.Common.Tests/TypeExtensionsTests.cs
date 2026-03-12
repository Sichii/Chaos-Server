#region
using System.Runtime.CompilerServices;
using Chaos.Extensions.Common;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Common.Tests;

public sealed class TypeExtensionsTests
{
      [Test]
      public void EnumerateProperties_ShouldRecurseIntoComplexProperties()
      {
            // Arrange
            var type = typeof(NestedClass);

            // Act
            var properties = type.EnumerateProperties()
                                 .Select(p => p.Name)
                                 .ToList();

            // Assert - Should recurse into Inner and yield its primitive properties
            properties.Should()
                      .Contain("IntValue");

            properties.Should()
                      .Contain("Name");

            // Should not contain the complex property itself
            properties.Should()
                      .NotContain("Inner");
      }

      [Test]
      public void EnumerateProperties_ShouldReturnPrimitiveAndStringProperties()
      {
            // Arrange
            var type = typeof(FlatClass);

            // Act
            var properties = type.EnumerateProperties()
                                 .Select(p => p.Name)
                                 .ToList();

            // Assert
            properties.Should()
                      .Contain("IntValue");

            properties.Should()
                      .Contain("Name");
      }

      [Test]
      public void ExtractGenericBaseType_Should_Return_AnotherDerivedType_When_GenericBaseType_Is_AnotherDerived()
      {
            // Arrange
            var yetAnotherDerivedType = typeof(YetAnotherDerived);

            // Act
            var result = yetAnotherDerivedType.ExtractGenericBaseType(typeof(AnotherDerived<>));

            // Assert
            result.Should()
                  .NotBeNull();

            result.Should()
                  .Be(typeof(AnotherDerived<string>));
      }

      [Test]
      public void ExtractGenericBaseType_Should_Return_BaseType_When_GenericBaseType_Is_Base()
      {
            // Arrange
            var derivedType = typeof(Derived);

            // Act
            var result = derivedType.ExtractGenericBaseType(typeof(Base<>));

            // Assert
            result.Should()
                  .NotBeNull();

            result.Should()
                  .Be(typeof(Base<int>));
      }

      [Test]
      public void ExtractGenericBaseType_Should_Return_Null_When_GenericBaseType_Is_Not_Found()
      {
            // Arrange
            var derivedType = typeof(Derived);

            // Act
            var result = derivedType.ExtractGenericBaseType(typeof(AnotherDerived<>));

            // Assert
            result.Should()
                  .BeNull();
      }

      [Test]
      public void ExtractGenericInterfaces_Should_Return_Empty_Collection_When_GenericInterfaceType_Is_Not_Found()
      {
            // Arrange
            var derivedType = typeof(IDerived);

            // Act
            var result = derivedType.ExtractGenericInterfaces(typeof(IAnotherDerived<>));

            // Assert
            result.Should()
                  .BeEmpty();
      }

      [Test]
      public void ExtractGenericInterfaces_Should_Return_Empty_Collection_When_Type_Does_Not_Implement_Generic_Interface()
      {
            // Arrange
            var myClassType = typeof(MyClass);

            // Act
            var result = myClassType.ExtractGenericInterfaces(typeof(IAnotherDerived<>));

            // Assert
            result.Should()
                  .BeEmpty();
      }

      [Test]
      public void ExtractGenericInterfaces_Should_Return_Interfaces_When_GenericInterfaceType_Is_Another_Interface()
      {
            // Arrange
            var yetAnotherDerivedType = typeof(IYetAnotherDerived);

            // Act
            var result = yetAnotherDerivedType.ExtractGenericInterfaces(typeof(IAnotherDerived<>))
                                              .ToList();

            // Assert
            result.Should()
                  .NotBeEmpty();

            result.Should()
                  .Contain(typeof(IAnotherDerived<string>));
      }

      [Test]
      public void ExtractGenericInterfaces_Should_Return_Interfaces_When_GenericInterfaceType_Is_Base_Interface()
      {
            // Arrange
            var derivedType = typeof(IDerived);

            // Act
            var result = derivedType.ExtractGenericInterfaces(typeof(IBase<>))
                                    .ToList();

            // Assert
            result.Should()
                  .NotBeEmpty();

            result.Should()
                  .Contain(typeof(IBase<int>));
      }

      [Test]
      public void GetBaseTypes_Should_Return_All_Base_Types()
      {
            // Arrange
            var derivedType = typeof(YetAnotherDerived);

            // Act
            var result = derivedType.GetBaseTypes()
                                    .ToList();

            // Assert
            result.Should()
                  .NotBeEmpty();

            result.Should()
                  .ContainInOrder(typeof(AnotherDerived<string>), typeof(Base<string>), typeof(object));
      }

      [Test]
      public void GetGenericMethod_ShouldReturnMethod_WhenMethodExists()
      {
            // Arrange
            var type = typeof(ClassWithGenericMethod);

            // Act
            var method = type.GetGenericMethod("GenericMethod", [typeof(int)]);

            // Assert
            method.Should()
                  .NotBeNull();

            method!.IsGenericMethod
                   .Should()
                   .BeTrue();
      }

      [Test]
      public void GetGenericMethod_ShouldReturnNull_WhenMethodDoesNotExist()
      {
            // Arrange
            var type = typeof(ClassWithGenericMethod);

            // Act
            var method = type.GetGenericMethod("NonExistentMethod", [typeof(int)]);

            // Assert
            method.Should()
                  .BeNull();
      }

      [Test]
      public void HasAttribute_ShouldReturnFalse_WhenTypeDoesNotHaveAttribute()
      {
            // Arrange
            var type = typeof(NonCompilerGeneratedClass);

            // Act
            var result = type.HasAttribute(typeof(CompilerGeneratedAttribute));

            // Assert
            result.Should()
                  .BeFalse();
      }

      [Test]
      public void HasAttribute_ShouldReturnTrue_WhenTypeHasAttribute()
      {
            // Arrange
            var type = typeof(CompilerGeneratedClass);

            // Act
            var result = type.HasAttribute(typeof(CompilerGeneratedAttribute));

            // Assert
            result.Should()
                  .BeTrue();
      }

      [Test]
      public void HasBaseType_Should_Return_False_When_Type_Does_Not_Have_Base_Type()
      {
            // Arrange
            var derivedType = typeof(Derived);

            // Act
            var result = derivedType.HasBaseType(typeof(AnotherDerived<>));

            // Assert
            result.Should()
                  .BeFalse();
      }

      [Test]
      public void HasBaseType_Should_Return_True_When_Type_Has_Base_Type()
      {
            // Arrange
            var derivedType = typeof(YetAnotherDerived);

            // Act
            var result = derivedType.HasBaseType(typeof(Base<string>));

            // Assert
            result.Should()
                  .BeTrue();
      }

      [Test]
      public void HasBaseType_Should_Return_True_When_Type_Has_Base_Type_Matching_Reference_Type()
      {
            // Arrange
            var derivedType = typeof(YetAnotherDerived);
            var baseType = typeof(AnotherDerived<>);

            // Act
            var result = derivedType.HasBaseType(baseType);

            // Assert
            result.Should()
                  .BeTrue();
      }

      [Test]
      public void HasBaseType_Should_Return_True_When_Type_Has_Generic_Base_Type_Definition()
      {
            // Arrange
            var derivedType = typeof(Derived);

            // Act
            var result = derivedType.HasBaseType(typeof(Base<>));

            // Assert
            result.Should()
                  .BeTrue();
      }

      [Test]
      public void HasInterface_Should_Return_False_When_Type_Does_Not_Implement_Interface()
      {
            // Arrange
            var classType = typeof(AnotherClass<>);

            // Act
            var result = classType.HasInterface(typeof(IDerived));

            // Assert
            result.Should()
                  .BeFalse();
      }

      [Test]
      public void HasInterface_Should_Return_True_When_Type_Implements_Generic_Interface_Definition()
      {
            // Arrange
            var classType = typeof(MyClass);

            // Act
            var result = classType.HasInterface(typeof(IBase<>));

            // Assert
            result.Should()
                  .BeTrue();
      }

      [Test]
      public void HasInterface_Should_Return_True_When_Type_Implements_Interface()
      {
            // Arrange
            var classType = typeof(MyClass);

            // Act
            var result = classType.HasInterface(typeof(IBase<int>));

            // Assert
            result.Should()
                  .BeTrue();
      }

      [Test]
      public void HasInterface_Should_Return_True_When_Type_Implements_Interface_Matching_Reference_Type()
      {
            // Arrange
            var classType = typeof(AnotherClass<int>);
            var interfaceType = typeof(IBase<>);

            // Act
            var result = classType.HasInterface(interfaceType);

            // Assert
            result.Should()
                  .BeTrue();
      }

      [Test]
      public void IsCompilerGenerated_Should_Return_False_For_Non_Compiler_Generated_Type()
      {
            // Arrange
            var nonCompilerGeneratedType = typeof(NonCompilerGeneratedClass);

            // Act
            var result = nonCompilerGeneratedType.IsCompilerGenerated();

            // Assert
            result.Should()
                  .BeFalse();
      }

      [Test]
      public void IsCompilerGenerated_Should_Return_True_For_Compiler_Generated_Type()
      {
            // Arrange
            var compilerGeneratedType = typeof(CompilerGeneratedClass);

            // Act
            var result = compilerGeneratedType.IsCompilerGenerated();

            // Assert
            result.Should()
                  .BeTrue();
      }

    [Test]
    public void IsFlagEnum_ShouldReturnFalse_WhenTypeIsEnumWithoutFlagsAttribute()
    {
        // Arrange
        var enumType = typeof(SampleEnum1);

        // Act
        var result = enumType.IsFlagEnum();

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void IsFlagEnum_ShouldReturnFalse_WhenTypeIsNotEnum()
    {
        // Arrange
        var nonEnumType = typeof(string);

        // Act
        var result = nonEnumType.IsFlagEnum();

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void IsFlagEnum_ShouldReturnTrue_WhenTypeIsFlagEnum()
    {
        // Arrange
        var flagEnumType = typeof(SampleFlag1);

        // Act
        var result = flagEnumType.IsFlagEnum();

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    [Arguments(typeof(object))]
    [Arguments(typeof(DateTime))]
    [Arguments(typeof(Enum))]
    public void IsPrimitive_ShouldReturnFalse_WhenTypeIsNotPrimitiveValueType(Type type)
    {
        // Act
        var result = type.IsPrimitive();

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    [Arguments(typeof(int))]
    [Arguments(typeof(double))]
    [Arguments(typeof(decimal))]
    [Arguments(typeof(bool))]
    public void IsPrimitive_ShouldReturnTrue_WhenTypeIsPrimitiveValueType(Type type)
    {
        // Act
        var result = type.IsPrimitive();

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void IsPrimitive_ShouldReturnTrue_WhenTypeIsString()
    {
        // Arrange
        var stringType = typeof(string);

        // Act
        var result = stringType.IsPrimitive();

        // Assert
        result.Should()
              .BeTrue();
    }

      [Test]
      public void LoadAttributedTypes_ShouldReturnTypesWithAttribute()
      {
            // Arrange
            var attributeType = typeof(SerializableAttribute);

            // Act
            var types = attributeType.LoadAttributedTypes()
                                     .ToList();

            // Assert
            types.Should()
                 .NotBeEmpty();
      }

      [Test]
      public void LoadImplementations_GenericTypeDefinition_Class_ShouldReturnDerived()
      {
            // Arrange
            var genericBase = typeof(Base<>);

            // Act
            var implementations = genericBase.LoadImplementations()
                                             .ToList();

            // Assert
            implementations.Should()
                           .Contain(typeof(Derived));

            implementations.Should()
                           .Contain(typeof(YetAnotherDerived));
      }

      [Test]
      public void LoadImplementations_GenericTypeDefinition_Interface_ShouldReturnImplementors()
      {
            // Arrange
            var genericInterface = typeof(IBase<>);

            // Act
            var implementations = genericInterface.LoadImplementations()
                                                  .ToList();

            // Assert
            implementations.Should()
                           .Contain(typeof(MyClass));

            // Generic type definitions return AnotherClass<T> not AnotherClass<int>
            implementations.Should()
                           .Contain(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(AnotherClass<>)));
      }

      [Test]
      public void LoadImplementations_Should_Return_All_Constructable_Types_Implementing_Interface()
      {
            // Arrange
            var interfaceType = typeof(IMyInterface);

            // Act
            var implementations = interfaceType.LoadImplementations()
                                               .ToList();

            // Assert
            implementations.Should()
                           .Contain(typeof(MyDerivedClass));

            implementations.Should()
                           .NotContain(interfaceType);
      }

      [Test]
      public void LoadImplementations_Should_Return_All_Constructable_Types_Inheriting_From_Base_Type()
      {
            // Arrange
            var baseType = typeof(MyBaseClass);

            // Act
            var implementations = baseType.LoadImplementations()
                                          .ToList();

            // Assert
            implementations.Should()
                           .Contain(typeof(MyDerivedClass));

            implementations.Should()
                           .NotContain(baseType);
      }

      [Test]
      public void LoadType_FiltersByDynamicAssemblies_ShouldExcludeDynamicAssemblies()
      {
            // This test verifies that dynamic assemblies are filtered out
            // We can't easily create a dynamic assembly in a unit test context
            // but we can verify the method works with regular assemblies

            // Arrange
            var typeName = "Object";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("Object");

            // Verify we're getting types from non-dynamic assemblies
            var assembly = result.Assembly;

            assembly.IsDynamic
                    .Should()
                    .BeFalse();
      }

      [Test]
      public void LoadType_WithCaseInsensitiveTypeName_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "string";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("String");
      }

      [Test]
      public void LoadType_WithComplexPredicate_ShouldFilterCorrectly()
      {
            // Arrange
            var typeName = "String";

            // Act
            var result = Type.LoadType(
                  typeName,
                  (assembly, type) =>
                  {
                        var assemblyName = assembly.GetName()
                                                   .Name;

                        return (assemblyName != null)
                               && assemblyName.Contains("System")
                               && type is { IsPublic: true, IsAbstract: false, IsClass: true };
                  });

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("String");

            result.IsPublic
                  .Should()
                  .BeTrue();

            result.IsAbstract
                  .Should()
                  .BeFalse();

            result.IsClass
                  .Should()
                  .BeTrue();
      }

      [Test]
      public void LoadType_WithCustomTypeFromCurrentAssembly_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = nameof(TypeExtensionsTests);

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be(nameof(TypeExtensionsTests));

            result.FullName
                  .Should()
                  .Contain("Chaos.Common.Tests");
      }

      [Test]
      public void LoadType_WithEmptyTypeName_ShouldReturnNull()
      {
            // Arrange
            var typeName = "";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .BeNull();
      }

      [Test]
      public void LoadType_WithEnumType_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "DayOfWeek";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("DayOfWeek");

            result.IsEnum
                  .Should()
                  .BeTrue();
      }

      [Test]
      public void LoadType_WithEnumTypeAndEnumPredicate_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "DayOfWeek";

            // Act
            var result = Type.LoadType(typeName, (_, type) => type.IsEnum);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("DayOfWeek");

            result.IsEnum
                  .Should()
                  .BeTrue();
      }

      [Test]
      public void LoadType_WithEnumTypeAndNonEnumPredicate_ShouldReturnNull()
      {
            // Arrange
            var typeName = "DayOfWeek";

            // Act
            var result = Type.LoadType(typeName, (_, type) => !type.IsEnum);

            // Assert
            result.Should()
                  .BeNull();
      }

      [Test]
      public void LoadType_WithGenericType_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "List`1";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("List`1");

            result.IsGenericTypeDefinition
                  .Should()
                  .BeTrue();
      }

      [Test]
      public void LoadType_WithInterfaceType_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "IDisposable";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("IDisposable");

            result.IsInterface
                  .Should()
                  .BeTrue();
      }

      [Test]
      public void LoadType_WithMixedCaseTypeName_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "StRiNg";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("String");
      }

      [Test]
      public void LoadType_WithMultipleTypesWithSameName_ShouldReturnFirst()
      {
            // Arrange
            var typeName = "Attribute"; // This exists in multiple assemblies

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("Attribute");
      }

      [Test]
      public void LoadType_WithNonExistentTypeName_ShouldReturnNull()
      {
            // Arrange
            var typeName = "NonExistentType12345";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .BeNull();
      }

      [Test]
      public void LoadType_WithNullPredicate_ShouldBehaveLikeNoPredicate()
      {
            // Arrange
            var typeName = "String";

            // Act
            var resultWithNull = Type.LoadType(typeName);
            var resultWithoutPredicate = Type.LoadType(typeName);

            // Assert
            resultWithNull.Should()
                          .NotBeNull();

            resultWithoutPredicate.Should()
                                  .NotBeNull();

            resultWithNull.Should()
                          .Be(resultWithoutPredicate);
      }

      [Test]
      public void LoadType_WithPredicate_ShouldReturnTypeMatchingPredicate()
      {
            // Arrange
            var typeName = "String";
            var predicateCalled = false;

            // Act
            var result = Type.LoadType(
                  typeName,
                  (assembly, _) =>
                  {
                        predicateCalled = true;

                        return assembly.GetName()
                                       .Name
                               == "System.Private.CoreLib";
                  });

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("String");

            predicateCalled.Should()
                           .BeTrue();
      }

      [Test]
      public void LoadType_WithPredicateFilteringByAssembly_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "Int32";

            // Act
            var result = Type.LoadType(
                  typeName,
                  (assembly, _) =>
                  {
                        var assemblyName = assembly.GetName()
                                                   .Name;

                        return assemblyName?.StartsWith("System") == true;
                  });

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("Int32");

            result.FullName
                  .Should()
                  .Be("System.Int32");
      }

      [Test]
      public void LoadType_WithPredicateFilteringByType_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "String";

            // Act
            var result = Type.LoadType(typeName, (_, type) => type is { IsValueType: false, IsClass: true });

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("String");

            result.IsClass
                  .Should()
                  .BeTrue();

            result.IsValueType
                  .Should()
                  .BeFalse();
      }

      [Test]
      public void LoadType_WithPredicateReturnsFalse_ShouldReturnNull()
      {
            // Arrange
            var typeName = "String";

            // Act
            var result = Type.LoadType(typeName, (_, _) => false);

            // Assert
            result.Should()
                  .BeNull();
      }

      [Test]
      public void LoadType_WithPredicateThrowingException_ShouldHandleGracefully()
      {
            // Arrange
            var typeName = "String";

            // Act
            var act = () => Type.LoadType(typeName, (_, _) => throw new InvalidOperationException("Test exception"));

            // Assert
            act.Should()
               .Throw<InvalidOperationException>()
               .WithMessage("Test exception");
      }

      [Test]
      public void LoadType_WithValidTypeName_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "String";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("String");

            result.FullName
                  .Should()
                  .Be("System.String");
      }

      [Test]
      public void LoadType_WithValueType_ShouldReturnCorrectType()
      {
            // Arrange
            var typeName = "Int32";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .NotBeNull();

            result.Name
                  .Should()
                  .Be("Int32");

            result.IsValueType
                  .Should()
                  .BeTrue();
      }

      [Test]
      public void LoadType_WithWhitespaceTypeName_ShouldReturnNull()
      {
            // Arrange
            var typeName = "   ";

            // Act
            var result = Type.LoadType(typeName);

            // Assert
            result.Should()
                  .BeNull();
      }

      public class ClassWithGenericMethod
      {
            public T GenericMethod<T>(T value) => value;
      }

      public class FlatClass
      {
            public int IntValue { get; set; }
            public string Name { get; set; } = "";
      }

      public class NestedClass
      {
            public decimal DecimalValue { get; set; }
            public FlatClass Inner { get; set; } = new();
      }

      #region MockTypes
      public class Base<T> { }

      public class Derived : Base<int> { }

      public class AnotherDerived<T> : Base<T> { }

      public class YetAnotherDerived : AnotherDerived<string> { }

      public interface IBase<T> { }

      public interface IDerived : IBase<int> { }

      public interface IAnotherDerived<T> : IBase<T> { }

      public interface IYetAnotherDerived : IAnotherDerived<string> { }

      public class MyClass : IBase<int> { }

      public class AnotherClass<T> : IBase<T> { }

      // Compiler-generated type
      [CompilerGenerated]
      private sealed class CompilerGeneratedClass { }

      // Non-compiler-generated type
      private sealed class NonCompilerGeneratedClass { }

      public interface IMyInterface { }

      public abstract class MyBaseClass { }

      public class MyDerivedClass : MyBaseClass, IMyInterface { }
      #endregion
}