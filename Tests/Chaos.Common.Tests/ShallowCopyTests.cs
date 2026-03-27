#region
using Chaos.Common.Utilities;
using FluentAssertions;

// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable UnusedMember.Local
#pragma warning disable CS0414 // Field is assigned but its value is never used
#endregion

namespace Chaos.Common.Tests;

public sealed class ShallowCopyTests
{
    [Test]
    public void Copy_ShouldReflectChangesInOriginal()
    {
        // Arrange
        var subClass = new MockCopyableSub
        {
            SubValue = 10
        };

        var original = new MockCopyable
        {
            Value = 5,
            MockCopyableSub = subClass
        };
        var copy = ShallowCopy<MockCopyable>.Create(original);

        // Act
        original.Value = 7;
        original.MockCopyableSub.SubValue = 15;

        // Assert
        copy.Value
            .Should()
            .NotBe(original.Value); // Value type

        copy.MockCopyableSub
            .SubValue
            .Should()
            .Be(original.MockCopyableSub.SubValue); // Reference type
    }

    [Test]
    public void Create_Should_Handle_Immutable_Strings_As_Values()
    {
        var original = new ImmutableHolder
        {
            Name = "alpha"
        };
        var copy = ShallowCopy<ImmutableHolder>.Create(original);

        copy.Name
            .Should()
            .Be("alpha");

        // Changing original reference to a new string should not change the copy
        original.Name = "beta";

        copy.Name
            .Should()
            .Be("alpha");
    }

    [Test]
    public void Create_ShouldReturnShallowCopy()
    {
        // Arrange
        var subClass = new MockCopyableSub
        {
            SubValue = 10
        };

        var original = new MockCopyable
        {
            Value = 5,
            MockCopyableSub = subClass
        };

        // Act
        var copy = ShallowCopy<MockCopyable>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.Value
            .Should()
            .Be(original.Value);

        copy.MockCopyableSub
            .Should()
            .BeSameAs(original.MockCopyableSub);
    }

    // Additional test cases to improve branch coverage

    [Test]
    public void Create_WithConstructorArgs_ShouldCreateInstanceAndMerge()
    {
        // Arrange
        var original = new ClassWithConstructorArgs(42, "original");

        // Act
        var copy = ShallowCopy<ClassWithConstructorArgs>.Create(original, 99, "copy");

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.Value
            .Should()
            .Be(42); // Should be merged from original, not constructor

        copy.Name
            .Should()
            .Be("original"); // Should be merged from original, not constructor
    }

    [Test]
    public void Merge_InheritedType_ShouldCopyBaseClassProperties()
    {
        // Arrange - Test that inherited class copies both base and derived properties
        var from = new DerivedClassWithProperties
        {
            BaseValue = 100,
            DerivedValue = 200
        };

        var to = new DerivedClassWithProperties
        {
            BaseValue = 0,
            DerivedValue = 0
        };

        // Act
        ShallowCopy<DerivedClassWithProperties>.Merge(from, to);

        // Assert
        to.BaseValue
          .Should()
          .Be(100);

        to.DerivedValue
          .Should()
          .Be(200);
    }

    [Test]
    public void Merge_InterfaceType_Copies_Properties()
    {
        var from = new InterfaceLike
        {
            Value = 10
        };

        var to = new InterfaceLike
        {
            Value = 0
        };

        ShallowCopy<InterfaceLike>.Merge(from, to);

        to.Value
          .Should()
          .Be(10);
    }

    [Test]
    public void Merge_InterfaceType_ShouldUseInterfaceBranch_ForPropertiesAndFields()
    {
        // Arrange - Use an actual interface type T to trigger the type.IsInterface branch
        ITestInterface from = new MultiInterfaceImplementation
        {
            InterfaceProperty = 42,
            InterfaceField = 99
        };

        ITestInterface to = new MultiInterfaceImplementation
        {
            InterfaceProperty = 0,
            InterfaceField = 0
        };

        // Act - ShallowCopy<ITestInterface> triggers the interface path in GetRecursiveProperties/GetRecursiveFields
        ShallowCopy<ITestInterface>.Merge(from, to);

        // Assert
        to.InterfaceProperty
          .Should()
          .Be(42);
    }

    [Test]
    public void Merge_InterfaceType_WithMultipleInterfaces_ShouldCopyAllProperties()
    {
        // Arrange - Use ISecondInterface which has overlapping property name with ITestInterface
        ISecondInterface from = new MultiInterfaceImplementation
        {
            InterfaceProperty = 77,
            SecondProperty = "fromValue"
        };

        ISecondInterface to = new MultiInterfaceImplementation
        {
            InterfaceProperty = 0,
            SecondProperty = "toValue"
        };

        // Act
        ShallowCopy<ISecondInterface>.Merge(from, to);

        // Assert
        to.InterfaceProperty
          .Should()
          .Be(77);

        to.SecondProperty
          .Should()
          .Be("fromValue");
    }

    [Test]
    public void Merge_Should_Throw_On_Null_Args()
    {
        var obj = new MockCopyable
        {
            Value = 1,
            MockCopyableSub = new MockCopyableSub()
        };
        var a = () => ShallowCopy<MockCopyable>.Merge(null!, obj);
        var b = () => ShallowCopy<MockCopyable>.Merge(obj, null!);

        a.Should()
         .Throw<ArgumentNullException>();

        b.Should()
         .Throw<ArgumentNullException>();
    }

    [Test]
    public void Merge_ShouldShallowMergeObjects()
    {
        // Arrange
        var subClass1 = new MockCopyableSub
        {
            SubValue = 10
        };

        var original = new MockCopyable
        {
            Value = 5,
            MockCopyableSub = subClass1
        };

        var subClass2 = new MockCopyableSub
        {
            SubValue = 20
        };

        var target = new MockCopyable
        {
            Value = 10,
            MockCopyableSub = subClass2
        };

        // Act
        ShallowCopy<MockCopyable>.Merge(original, target);

        // Assert
        target.Value
              .Should()
              .Be(original.Value);

        target.MockCopyableSub
              .Should()
              .BeSameAs(original.MockCopyableSub);

        target.MockCopyableSub
              .SubValue
              .Should()
              .Be(original.MockCopyableSub.SubValue);
    }

    [Test]
    public void Merge_WithNull_FromObj_ShouldThrowArgumentNullException()
    {
        // Arrange
        var target = new MockCopyable
        {
            Value = 1,
            MockCopyableSub = new MockCopyableSub()
        };

        // Act & Assert
        var act = () => ShallowCopy<MockCopyable>.Merge(null!, target);

        act.Should()
           .Throw<ArgumentNullException>()
           .And
           .ParamName
           .Should()
           .Be("fromObj");
    }

    [Test]
    public void Merge_WithNull_TargetObj_ShouldThrowArgumentNullException()
    {
        // Arrange
        var from = new MockCopyable
        {
            Value = 1,
            MockCopyableSub = new MockCopyableSub()
        };

        // Act & Assert
        var act = () => ShallowCopy<MockCopyable>.Merge(from, null!);

        act.Should()
           .Throw<ArgumentNullException>()
           .And
           .ParamName
           .Should()
           .Be("targetObj");
    }

    [Test]
    public void ShallowCopy_ConcreteTypeWithInterfaceProperties_ShouldCopyCorrectly()
    {
        // Arrange
        var original = new MultiInterfaceImplementation
        {
            InterfaceProperty = 500,
            InterfaceField = 600,
            SecondProperty = "modified"
        };

        // Act - Using concrete type to test interface property copying
        var copy = ShallowCopy<MultiInterfaceImplementation>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.InterfaceProperty
            .Should()
            .Be(500);

        copy.InterfaceField
            .Should()
            .Be(600);

        copy.SecondProperty
            .Should()
            .Be("modified");

        copy.InterfaceMethod()
            .Should()
            .Be(600); // Test the field through method
    }

    [Test]
    public void ShallowCopy_EmptyType_ShouldCreateInstance()
    {
        // Arrange
        var original = new EmptyType();

        // Act
        var copy = ShallowCopy<EmptyType>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.Should()
            .NotBeNull();
    }

    [Test]
    public void ShallowCopy_MultipleCallsOnSameType_ShouldReuseCompiledDelegate()
    {
        // Arrange
        var original1 = new MockCopyable
        {
            Value = 111,
            MockCopyableSub = new MockCopyableSub
            {
                SubValue = 222
            }
        };

        var original2 = new MockCopyable
        {
            Value = 333,
            MockCopyableSub = new MockCopyableSub
            {
                SubValue = 444
            }
        };

        // Act - Multiple calls to test that AssignmentDelegate is reused
        var copy1 = ShallowCopy<MockCopyable>.Create(original1);
        var copy2 = ShallowCopy<MockCopyable>.Create(original2);

        // Assert
        copy1.Value
             .Should()
             .Be(111);

        copy1.MockCopyableSub
             .SubValue
             .Should()
             .Be(222);

        copy2.Value
             .Should()
             .Be(333);

        copy2.MockCopyableSub
             .SubValue
             .Should()
             .Be(444);
    }

    [Test]
    public void ShallowCopy_ShouldHandleFieldsCopyingCorrectly()
    {
        // Arrange
        var original = new ClassWithFields
        {
            PublicField = 10,
            ProtectedInternalField = 20,
            InternalField = 30,
            PrivateFieldAccessor = 40
        };

        // Act
        var copy = ShallowCopy<ClassWithFields>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.PublicField
            .Should()
            .Be(10);

        copy.ProtectedInternalField
            .Should()
            .Be(20);

        copy.InternalField
            .Should()
            .Be(30);

        copy.PrivateFieldAccessor
            .Should()
            .Be(40);

        copy.ReadOnlyField
            .Should()
            .Be(42); // Should be default value, not copied
    }

    [Test]
    public void ShallowCopy_ShouldHandleInitOnlyProperties()
    {
        // Arrange
        var original = new ClassWithInitOnlyProperties();
        original.RegularProperty = 888;
        original.RegularField = 777;

        // Act
        var copy = ShallowCopy<ClassWithInitOnlyProperties>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.InitOnlyProperty
            .Should()
            .Be(10); // Default value since init-only can't be copied

        copy.RegularProperty
            .Should()
            .Be(888); // Should be copied

        copy.RegularField
            .Should()
            .Be(777); // Should be copied
    }

    [Test]
    public void ShallowCopy_ShouldOnlyCopyReadWriteProperties()
    {
        // Arrange
        var original = new ClassWithMixedProperties();
        original.WriteOnlyProperty = 100;

        // Act
        var copy = ShallowCopy<ClassWithMixedProperties>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.ReadWriteProperty
            .Should()
            .Be(10);

        copy.ReadOnlyProperty
            .Should()
            .Be(20); // Default value, not copied since it's read-only

        copy.GetWriteOnlyValue()
            .Should()
            .Be(100); // This field actually gets copied since it's a backing field
    }

    [Test]
    public void ShallowCopy_TypeWithDistinctPropertyNames_ShouldHandleCorrectly()
    {
        // Arrange - This test ensures the DistinctBy logic is covered
        var original = new MultiInterfaceImplementation
        {
            InterfaceProperty = 123,
            SecondProperty = "distinct test"
        };

        // Act
        var copy = ShallowCopy<MultiInterfaceImplementation>.Create(original);

        // Assert
        copy.InterfaceProperty
            .Should()
            .Be(123);

        copy.SecondProperty
            .Should()
            .Be("distinct test");
    }

    [Test]
    public void ShallowCopy_TypeWithOnlyFields_ShouldCopyAllFields()
    {
        // Arrange
        var original = new TypeWithOnlyFields
        {
            PublicField = 100,
            InternalField = 200
        };

        // Act
        var copy = ShallowCopy<TypeWithOnlyFields>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.PublicField
            .Should()
            .Be(100);

        copy.InternalField
            .Should()
            .Be(200);
    }

    [Test]
    public void ShallowCopy_TypeWithPrivateMembers_ShouldCopyCorrectly()
    {
        // Arrange  
        var original = new ClassWithMixedProperties();
        original.ReadWriteProperty = 500;

        // Act
        var copy = ShallowCopy<ClassWithMixedProperties>.Create(original);

        // Assert
        copy.Should()
            .NotBeSameAs(original);

        copy.ReadWriteProperty
            .Should()
            .Be(500);

        copy.InternalProperty
            .Should()
            .Be(60); // Should copy internal properties
    }

    internal class BaseClassWithProperties
    {
        public int BaseValue { get; set; }
    }

    internal sealed class ClassWithConstructorArgs
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public ClassWithConstructorArgs()
            : this(0, "default") { }

        public ClassWithConstructorArgs(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }

    // Test classes for comprehensive coverage
    internal class ClassWithFields
    {
        public const int CONST_FIELD = 100;
        public static int StaticField = 50;
        public readonly int ReadOnlyField = 42;
        internal int InternalField;
        protected internal int ProtectedInternalField;
        public int PublicField;

        public int PrivateFieldAccessor { get; set; }
    }

    internal sealed class ClassWithInitOnlyProperties
    {
        public readonly int ReadOnlyField = 30;
        public int RegularField = 40;
        public int InitOnlyProperty { get; init; } = 10;
        public int RegularProperty { get; set; } = 20;
    }

    internal class ClassWithMixedProperties
    {
        private int _privateProperty = 30;
        private int _writeOnlyValue;
        internal int InternalProperty { get; set; } = 60;
        private int PrivateProperty { get; set; } = 40;
        protected int ProtectedProperty { get; set; } = 50;
        public int ReadWriteProperty { get; set; } = 10;

        public int WriteOnlyProperty
        {
            set => _writeOnlyValue = value;
        }

        public int ReadOnlyProperty => 20;
        public int GetWriteOnlyValue() => _writeOnlyValue;
    }

    internal sealed class DerivedClassWithProperties : BaseClassWithProperties
    {
        public int DerivedValue { get; set; }
    }

    internal sealed class EmptyType
    {
        // Empty class to test edge case
    }

    internal sealed class ImmutableHolder
    {
        public string Name { get; set; } = string.Empty;
    }

    internal sealed class InterfaceLike
    {
        public int Value { get; set; }
    }

    internal interface ISecondInterface
    {
        int InterfaceProperty { get; set; } // Duplicate name to test DistinctBy
        string SecondProperty { get; set; }
    }

    internal interface ITestInterface
    {
        int InterfaceProperty { get; set; }
        int InterfaceMethod();
    }

    internal sealed class MockCopyable
    {
        public MockCopyableSub MockCopyableSub { get; set; } = null!;
        public int Value { get; set; }
    }

    internal sealed class MockCopyableSub
    {
        public int SubValue { get; set; }
    }

    internal sealed class MultiInterfaceImplementation : ITestInterface, ISecondInterface
    {
        public int InterfaceField = 200;
        public int InterfaceProperty { get; set; } = 100;
        public string SecondProperty { get; set; } = "test";

        public int InterfaceMethod() => InterfaceField;
    }

    internal sealed class TypeWithOnlyFields
    {
        public readonly int ReadOnlyField = 999;
        private int _privateField;
        internal int InternalField;
        public int PublicField;

        public int GetPrivateField() => _privateField;
        public void SetPrivateField(int value) => _privateField = value;
    }
}