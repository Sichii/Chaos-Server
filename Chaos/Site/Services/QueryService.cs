using System.Linq.Expressions;
using System.Reflection;
using Chaos.Common.Comparers;
using Chaos.Extensions.Common;
using Chaos.Site.Definitions;
using Chaos.Site.Models;

namespace Chaos.Site.Services;

public sealed class QueryService
{
    private readonly ConcurrentDictionary<string, Delegate> Selectors = new();
    private readonly ConcurrentDictionary<string, object> SortComparers = new();

    private IEnumerable<T> ApplyFilter<T>(IEnumerable<T> items, Dictionary<string, FilterModel> filterModels)
    {
        var predicate = CreateFilterPredicate<T>(filterModels);

        return items.Where(predicate);
    }

    private IOrderedEnumerable<T> ApplySort<T>(IEnumerable<T> items, SortModel model)
    {
        var comparer = CreateSortComparer<T>(model);

        if (items is IOrderedEnumerable<T> orderedEnumerable)
            if (model.Sort == SortType.Asc)
                return orderedEnumerable.ThenBy(comparer);
            else
                return orderedEnumerable.ThenByDescending(comparer);

        if (model.Sort == SortType.Asc)
            return items.OrderBy(comparer);

        return items.OrderByDescending(comparer);
    }

    private Func<T, bool> CreateFilterPredicate<T>(Dictionary<string, FilterModel> filterModels)
    {
        var predicates = filterModels.Select(
            kvp =>
            {
                var propertyName = kvp.Key;
                var filterModel = kvp.Value;

                //if there are multiple conditions in this filter, we need to combine them
                if (!filterModel.Conditions.IsNullOrEmpty())
                {
                    //create filter predicates for each of the sub conditions
                    var subPredicates = filterModel.Conditions.Select(condition => CreateSingleFilterPredicate<T>(propertyName, condition));

                    //depending on the operator, combine the sub predicates
                    return obj => filterModel.Operator switch
                    {
                        FilterGroupOperator.And => subPredicates.All(predicate => predicate(obj)),
                        FilterGroupOperator.Or  => subPredicates.Any(predicate => predicate(obj)),
                        _                       => throw new ArgumentOutOfRangeException()
                    };
                }

                //otherwise, create a filter predicate for this condition
                return CreateSingleFilterPredicate<T>(kvp.Key, kvp.Value);
            });

        return obj => predicates.All(predicate => predicate(obj));
    }

    private Func<T, bool> CreateNumberPredicate<T>(Func<T, object?> selector, FilterModel model)
    {
        var filterValue = model.FilterValue?.GetDecimal();

        return model.Operation switch
        {
            FilterOperator.Equals => obj =>
            {
                var val = GetDecimal(selector(obj));
                var ret = filterValue == val;

                return ret; //val?.Equals(filterValue) ?? false;
            },
            FilterOperator.NotEqual => obj =>
            {
                var val = GetDecimal(selector(obj));

                return !(val?.Equals(filterValue) ?? false);
            },
            FilterOperator.GreaterThan => obj =>
            {
                var val = GetDecimal(selector(obj));

                return val switch
                {
                    null => false,
                    _    => ((IComparable)val).CompareTo(filterValue) > 0
                };
            },
            FilterOperator.GreaterThanOrEqual => obj =>
            {
                var val = GetDecimal(selector(obj));

                return val switch
                {
                    null => false,
                    _    => ((IComparable)val).CompareTo(filterValue) >= 0
                };
            },
            FilterOperator.LessThan => obj =>
            {
                var val = GetDecimal(selector(obj));

                return val switch
                {
                    null => false,
                    _    => ((IComparable)val).CompareTo(filterValue) < 0
                };
            },
            FilterOperator.LessThanOrEqual => obj =>
            {
                var val = GetDecimal(selector(obj));

                return val switch
                {
                    null => false,
                    _    => ((IComparable)val).CompareTo(filterValue) <= 0
                };
            },
            FilterOperator.InRange => obj =>
            {
                var val = GetDecimal(selector(obj));
                var filterToValue = model.FilterToValue?.GetDecimal();

                return val switch
                {
                    null => false,
                    _    => (((IComparable)val).CompareTo(filterValue) >= 0) && (((IComparable)val).CompareTo(filterToValue) <= 0)
                };
            },
            FilterOperator.Blank    => obj => selector(obj) is null,
            FilterOperator.NotBlank => obj => selector(obj) is not null,
            _                       => throw new ArgumentOutOfRangeException()
        };
    }

    public IEnumerable<T> CreateQuery<T>(IEnumerable<T> items, GetRowsParams rowParams)
    {
        if (rowParams.FilterModel != null)
            items = ApplyFilter(items, rowParams.FilterModel);

        if (rowParams.SortModel != null)
        {
            var sortModel = rowParams.SortModel;

            foreach (var model in sortModel)
                items = ApplySort(items, model);
        }

        return items.Skip(rowParams.StartRow)
                    .Take(rowParams.EndRow - rowParams.StartRow);
    }

    private LambdaExpression CreateSafeNavigationSelector<T>(string propertyName)
    {
        var type = typeof(T);
        var paramExpression = Expression.Parameter(type);
        var propertyExpression = default(Expression);
        var subPropertyChain = GetPropertyPath(type, propertyName);

        foreach (var subProperty in subPropertyChain)
        {
            var property = type.GetProperty(subProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)!;

            //the current body
            var parentExpression = propertyExpression ?? paramExpression;

            //default value of current body
            var currentDefaultValue = Expression.Constant(GetDefaultValue(type), type);

            //access the given property on the given expression
            var memberAccess = (Expression)Expression.MakeMemberAccess(parentExpression, property);

            //next default value
            var nextDefaultValue = Expression.Constant(GetDefaultValue(memberAccess.Type), memberAccess.Type);

            if (memberAccess.Type.IsValueType && (subPropertyChain.Count > 1))
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(memberAccess.Type);
                memberAccess = Expression.Convert(memberAccess, nullableType);
                nextDefaultValue = Expression.Constant(null, nullableType);
            }

            //if the current body is null, return the default value for that type
            var equalsDefault = Expression.Equal(parentExpression, currentDefaultValue);

            //set the body to return null if the current body is null, otherwise return the member access
            propertyExpression = Expression.Condition(equalsDefault, nextDefaultValue, memberAccess);

            type = propertyExpression.Type;
        }

        var expressionType = typeof(Func<,>).MakeGenericType(typeof(T), type);

        return Expression.Lambda(expressionType, propertyExpression!, paramExpression);
    }

    private Func<T, object?> CreateSelector<T>(string propertyName)
    {
        var selectorExpression = CreateSafeNavigationSelector<T>(propertyName);
        var convert = Expression.Convert(selectorExpression.Body, typeof(object));
        var lambda = Expression.Lambda<Func<T, object?>>(convert, selectorExpression.Parameters);

        return lambda.Compile();
    }

    private Func<T, bool> CreateSingleFilterPredicate<T>(string propertyName, FilterModel model)
    {
        var identifier = $"{typeof(T).FullName}|{propertyName}";
        var selector = (Func<T, object?>)Selectors.GetOrAdd(identifier, _ => CreateSelector<T>(propertyName));

        if (model.FilterType == FilterType.Number)
            return CreateNumberPredicate(selector, model);

        return CreateTextPredicate(selector, model);
    }

    private IComparer<T> CreateSortComparer<T>(SortModel model)
    {
        return (IComparer<T>)SortComparers.GetOrAdd($"{typeof(T).FullName}|{model.ColId}", _ => InnerCreateSortComparer());

        IComparer<T> InnerCreateSortComparer()
        {
            var paramExpression = Expression.Parameter(typeof(T));

            // obj => obj?.Property?.SubProperty
            var selectorExpression = CreateSafeNavigationSelector<T>(model.ColId);

            // selectorExpression(obj)
            var invokeSelectorExpression = Expression.Invoke(selectorExpression, paramExpression);

            // (IComparable)selectorExpression(obj)
            var convertExpression = Expression.Convert(invokeSelectorExpression, typeof(IComparable));

            var lambda = Expression.Lambda<Func<T?, IComparable>>(convertExpression, paramExpression)
                                   .Compile();

            return LambdaComparer<T>.FromSelect(lambda);
        }
    }

    private Func<T, bool> CreateTextPredicate<T>(Func<T, object?> selector, FilterModel model)
    {
        var filterValue = model.FilterValue?.GetString();

        switch (model.Operation)
        {
            case FilterOperator.Contains:
                return obj =>
                {
                    var val = selector(obj);
                    var str = val?.ToString();

                    return str?.ContainsI(filterValue!) ?? false;
                };
            case FilterOperator.NotContains:
                return obj =>
                {
                    var val = selector(obj);
                    var str = val?.ToString();

                    return !(str?.ContainsI(filterValue!) ?? false);
                };
            case FilterOperator.Equals:
                return obj =>
                {
                    var val = selector(obj);
                    var str = val?.ToString();

                    return str?.EqualsI(filterValue!) ?? false;
                };
            case FilterOperator.NotEqual:
                return obj =>
                {
                    var val = selector(obj);
                    var str = val?.ToString();

                    return !(str?.EqualsI(filterValue!) ?? false);
                };
            case FilterOperator.StartsWith:
                return obj =>
                {
                    var val = selector(obj);
                    var str = val?.ToString();

                    return str?.StartsWithI(filterValue!) ?? false;
                };
            case FilterOperator.EndsWith:
                return obj =>
                {
                    var val = selector(obj);
                    var str = val?.ToString();

                    return str?.EndsWithI(filterValue!) ?? false;
                };
            case FilterOperator.Blank:
                return obj => string.IsNullOrWhiteSpace(
                    selector(obj)
                        ?.ToString());
            case FilterOperator.NotBlank:
                return obj => !string.IsNullOrWhiteSpace(
                    selector(obj)
                        ?.ToString());
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private decimal? GetDecimal(object? boxed)
        => boxed switch
        {
            null      => null,
            decimal d => d,
            _         => Convert.ToDecimal(boxed)
        };

    private object GetDefaultValue(Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type)!;

        return null!;
    }

    private List<string> GetPropertyPath(Type type, string propertyName)
    {
        var path = new List<string>();

        if (type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) is not null)
        {
            path.Add(propertyName);

            return path;
        }

        var complexSubProperties = type.GetProperties()
                                       .Where(prop => !prop.PropertyType.IsValueType && (prop.PropertyType != typeof(string)));

        foreach (var property in complexSubProperties)
        {
            var subPath = GetPropertyPath(property.PropertyType, propertyName);

            if (subPath.Count > 0)
            {
                path.Add(property.Name);
                path.AddRange(subPath);
            }
        }

        return path;
    }
}