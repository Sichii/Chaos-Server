#region
using System.Runtime.CompilerServices;
using FluentAssertions;
#endregion

// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable UnusedTypeParameter

namespace Chaos.Extensions.Common.Tests;

public sealed class TypeExtensionsTests
{
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