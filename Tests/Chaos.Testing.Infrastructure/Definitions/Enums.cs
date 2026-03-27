#region
using Chaos.Common.Attributes;
using Chaos.Common.CustomTypes;
#endregion

namespace Chaos.Testing.Infrastructure.Definitions;

public enum SampleEnum1
{
    Value1,
    Value2,
    Value3
}

public enum SampleEnum2
{
    Value1,
    Value2,
    Value3
}

[Flags]
public enum SampleFlag1
{
    Value1 = 1,
    Value2 = 2,
    Value3 = 4
}

public enum ColorEnum
{
    Red,
    Blue,
    Green
}

[Flags]
public enum ColorFlag
{
    Red = 1,
    Blue = 2,
    Green = 4
}

public enum SizeEnum
{
    Small,
    Medium,
    Large
}

[Flags]
public enum SizeFlag
{
    Small = 1,
    Medium = 2,
    Large = 4
}

// BigFlags marker types for testing
public sealed class TestFeatures : BigFlags<TestFeatures>
{
    public static BigFlagsValue<TestFeatures> Feature1;
    public static BigFlagsValue<TestFeatures> Feature2;
    public static BigFlagsValue<TestFeatures> Feature3;
    public static BigFlagsValue<TestFeatures> Feature4;

    static TestFeatures() => Initialize();
}

public sealed class TestPermissions : BigFlags<TestPermissions>
{
    public static BigFlagsValue<TestPermissions> Read;
    public static BigFlagsValue<TestPermissions> Write;
    public static BigFlagsValue<TestPermissions> Execute;
    public static BigFlagsValue<TestPermissions> Delete;

    static TestPermissions() => Initialize();
}

public sealed class TestExplicitIndices : BigFlags<TestExplicitIndices>
{
    [BitIndex(10)]
    public static BigFlagsValue<TestExplicitIndices> Flag10;

    [BitIndex(20)]
    public static BigFlagsValue<TestExplicitIndices> Flag20;

    [BitIndex(100)]
    public static BigFlagsValue<TestExplicitIndices> Flag100;

    public static BigFlagsValue<TestExplicitIndices> AutoFlag;

    static TestExplicitIndices() => Initialize();
}