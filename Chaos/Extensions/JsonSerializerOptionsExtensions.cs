using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Chaos.Extensions;

public static class JsonSerializerOptionsExtensions
{
    private static readonly Action<JsonSerializerOptions, IJsonTypeInfoResolver> SetTypeResolverAction;

    static JsonSerializerOptionsExtensions()
    {
        var optionsType = typeof(JsonSerializerOptions);
        var resolverType = typeof(IJsonTypeInfoResolver);

        var fieldInfo = optionsType.GetField("_typeInfoResolver", BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo == null)
            throw new InvalidOperationException("Could not find field '_typeInfoResolver' on type 'JsonSerializerOptions'");

        var optionsParam = Expression.Parameter(optionsType);
        var resolverParam = Expression.Parameter(resolverType);
        var field = Expression.Field(optionsParam, fieldInfo);

        var assignmentExp = Expression.Assign(field, resolverParam);

        SetTypeResolverAction = Expression
                                .Lambda<Action<JsonSerializerOptions, IJsonTypeInfoResolver>>(assignmentExp, optionsParam, resolverParam)
                                .Compile();
    }

    public static void SetTypeResolver(this JsonSerializerOptions options, IJsonTypeInfoResolver resolver)
        => SetTypeResolverAction(options, resolver);
}