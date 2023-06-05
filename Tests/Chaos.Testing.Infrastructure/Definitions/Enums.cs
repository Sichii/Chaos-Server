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