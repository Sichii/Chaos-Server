namespace Chaos.Site.Definitions;

public enum SortType
{
    Asc,
    Desc
}

public enum FilterType
{
    Text,
    Number
}

public enum FilterGroupOperator
{
    And,
    Or
}

public enum FilterOperator
{
    Contains,
    NotContains,
    Equals,
    NotEqual,
    StartsWith,
    EndsWith,
    Blank,
    NotBlank,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    InRange
}