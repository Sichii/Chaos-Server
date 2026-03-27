#region
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="Type" />.
/// </summary>
public static class Typetensions
{
    /// <summary>
    ///     Provides extension methods for <see cref="System.Type" />.
    /// </summary>
    extension(Type)
    {
        /// <summary>
        ///     Determines if a type is a flag enum.
        /// </summary>
        /// <typeparam name="T">
        ///     The type to check
        /// </typeparam>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if the provided type is an enum and has the <see cref="FlagsAttribute" /> attribute, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        [ExcludeFromCodeCoverage(Justification = "Tested by IsFlagEnum(Type)")]
        public static bool IsFlagEnum<T>() where T: Enum => IsFlagEnum(typeof(T));

        /// <summary>
        ///     Loads a type by name.
        /// </summary>
        public static Type? LoadType(string typeName, Func<Assembly, Type, bool>? predicate = null)
        {
            var type = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Where(a => !a.IsDynamic)
                                .SelectMany(a =>
                                {
                                    try
                                    {
                                        return a.GetTypes();
                                    } catch
                                    {
                                        return [];
                                    }
                                })
                                .FirstOrDefault(asmType
                                    => (predicate?.Invoke(asmType.Assembly, asmType) ?? true)
                                       && asmType.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

            return type;
        }
    }

    /// <param name="type">
    /// </param>
    extension(Type type)
    {
        /// <summary>
        ///     Recursively enumerates all properties on a complex object, only returning the PropertyInfo objects of primitive or
        ///     string properties
        /// </summary>
        public IEnumerable<PropertyInfo> EnumerateProperties()
        {
            foreach (var property in type.GetProperties())
                if (property.PropertyType is { IsClass: true, IsValueType: false, IsPrimitive: false }
                    && (property.PropertyType != typeof(string)))
                    foreach (var nestedProperty in property.PropertyType.EnumerateProperties())
                        yield return nestedProperty;
                else
                    yield return property;
        }

        /// <summary>
        ///     Extracts the generic base type that use a generic type definition within the hierarchy of the type.
        /// </summary>
        /// <param name="genericBaseType">
        ///     A generic type definition (non-interface). (The type of a generic without the type params specified)
        /// </param>
        /// <returns>
        /// </returns>
        public Type? ExtractGenericBaseType(Type genericBaseType)
        {
            var current = type;

            while (current != null)
            {
                if (current.IsGenericType && (current.GetGenericTypeDefinition() == genericBaseType))
                    return current;

                current = current.BaseType;
            }

            return null;
        }

        /// <summary>
        ///     Extracts all generic interfaces types from a generic type definition within the hierarchy of the type.
        /// </summary>
        /// <param name="genericInterfaceType">
        ///     A generic type definition of an interface. (The type of a generic without the type params specified)
        /// </param>
        public IEnumerable<Type> ExtractGenericInterfaces(Type genericInterfaceType)
        {
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
                if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == genericInterfaceType))
                    yield return @interface;
        }

        /// <summary>
        ///     Lazily yields all base types of a type
        /// </summary>
        public IEnumerable<Type> GetBaseTypes()
        {
            var current = type.BaseType;

            while (current != null)
            {
                yield return current;

                current = current.BaseType;
            }
        }

        /// <summary>
        ///     Gets a generic method from a type
        /// </summary>
        public MethodInfo? GetGenericMethod(string methodName, Type[] genericTypes)
        {
            var method = type.GetMethod(methodName);

            if (method == null)
                return null;

            return method.MakeGenericMethod(genericTypes);
        }

        /// <summary>
        ///     Determines whether the specified type has a custom attribute of the specified attribute type.
        /// </summary>
        /// <param name="attributeType">
        ///     The type of the attribute to search for.
        /// </param>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     when the type has the specified attribute, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        public bool HasAttribute(Type attributeType) => type.GetCustomAttribute(attributeType) is not null;

        /// <summary>
        ///     Determines whether a type inherits from the specified base type
        /// </summary>
        public bool HasBaseType(Type baseType)
        {
            var current = type;

            while (current != null)
            {
                if (baseType.IsGenericTypeDefinition)
                {
                    if (current.IsGenericType && (current.GetGenericTypeDefinition() == baseType))
                        return true;
                } else if (current == baseType)
                    return true;

                current = current.BaseType;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether a type inherits from an interface
        /// </summary>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if the type implements the interface, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        public bool HasInterface(Type interfaceType)
        {
            foreach (var iType in type.GetInterfaces())
                if (interfaceType.IsGenericTypeDefinition)
                {
                    if (iType.IsGenericType && (iType.GetGenericTypeDefinition() == interfaceType))
                        return true;
                } else if (iType == interfaceType)
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines if a type is a compiler generated type.
        /// </summary>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if the type is compiler generated, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        public bool IsCompilerGenerated() => type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;

        /// <summary>
        ///     Determines if a type is a flag enum.
        /// </summary>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if the provided type is an enum and has the <see cref="FlagsAttribute" /> attribute, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        public bool IsFlagEnum()
            => type.IsEnum
               && (type.GetCustomAttributes(typeof(FlagsAttribute), false)
                       .Length
                   != 0);

        /// <summary>
        ///     Determines if a type is a primitive type.
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsPrimitive()
            => (type == typeof(string)) || (type == typeof(decimal)) || type is { IsValueType: true, IsPrimitive: true };

        /// <summary>
        ///     Loads all types from the current application domain's assemblies that are annotated with the specified attribute
        ///     type.
        /// </summary>
        /// <returns>
        ///     An enumerable collection of types annotated with the specified attribute type
        /// </returns>
        public IEnumerable<Type> LoadAttributedTypes()
            => AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Where(a => !a.IsDynamic)
                        .SelectMany(a =>
                        {
                            try
                            {
                                return a.GetTypes();
                            } catch
                            {
                                return [];
                            }
                        })
                        .Where(asmType => asmType.HasAttribute(type));

        /// <summary>
        ///     Returns all constructable types that inherit from the specified type.
        /// </summary>
        public IEnumerable<Type> LoadImplementations()
        {
            var assemblyTypes = AppDomain.CurrentDomain
                                         .GetAssemblies()
                                         .Where(a => !a.IsDynamic)
                                         .SelectMany(a =>
                                         {
                                             try
                                             {
                                                 return a.GetTypes();
                                             } catch
                                             {
                                                 return [];
                                             }
                                         })
                                         .Where(asmType => asmType is { IsInterface: false, IsAbstract: false });

            if (type.IsGenericTypeDefinition)
                return assemblyTypes.Where(asmType => type.IsInterface ? asmType.HasInterface(type) : asmType.HasBaseType(type));

            return assemblyTypes.Where(asmType => asmType.IsAssignableTo(type));
        }
    }
}