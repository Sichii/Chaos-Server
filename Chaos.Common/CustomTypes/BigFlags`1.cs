#region
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Chaos.Common.Attributes;
#endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chaos.Common.CustomTypes;

/// <summary>
///     Abstract base class for defining named flag constants with automatic bit index assignment. Derive from this class
///     and define static fields of type BigFlagsValue&lt;TMarker&gt;. Fields will be automatically assigned sequential bit
///     indices starting from 0. Use [BitIndex(n)] attribute to explicitly specify a bit index.
/// </summary>
/// <typeparam name="TMarker">
///     A marker type (typically the derived class itself) to ensure type safety
/// </typeparam>
public abstract class BigFlags<TMarker> where TMarker: class
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<Type, bool> Initialized = new();

    // ReSharper disable once StaticMemberInGenericType
    private static readonly Lock Sync = new();

    // Cached metadata for fast lookups
    // ReSharper disable once StaticMemberInGenericType
    private static readonly List<string> CachedNames = [];
    private static readonly List<BigFlagsValue<TMarker>> CachedValues = [];
    private static readonly Dictionary<string, BigFlagsValue<TMarker>> CachedNameToValue = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, BigFlagsValue<TMarker>> CachedNameToValueIgnoreCase = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<BigFlagsValue<TMarker>, string> CachedValueToName = new();

    /// <summary>
    ///     Represents an empty value with no flags set
    /// </summary>
    public static readonly BigFlagsValue<TMarker> None = new(BigInteger.Zero);

    /// <summary>
    ///     Gets the name of a flag value, if it represents a single defined flag.
    /// </summary>
    /// <param name="value">
    ///     The flag value to get the name for
    /// </param>
    /// <returns>
    ///     The name of the flag, or null if the value doesn't match a single defined flag
    /// </returns>
    public static string? GetName(BigFlagsValue<TMarker> value) => CachedValueToName.GetValueOrDefault(value);

    /// <summary>
    ///     Returns the names of all defined flags in the derived class.
    /// </summary>
    public static IEnumerable<string> GetNames() => CachedNames;

    /// <summary>
    ///     Returns a dictionary mapping flag names to their values.
    /// </summary>
    public static Dictionary<string, BigFlagsValue<TMarker>> GetNameValueMap() => new(CachedNameToValue);

    /// <summary>
    ///     Returns all defined flag values in the derived class.
    /// </summary>
    public static IEnumerable<BigFlagsValue<TMarker>> GetValues() => CachedValues;

    //static BigFlags() => Initialize();

    protected static void Initialize()
    {
        lock (Sync)
        {
            var type = typeof(TMarker);

            if (!Initialized.TryAdd(type, true))
                return;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                             .Where(f => f.FieldType == typeof(BigFlagsValue<TMarker>))
                             .OrderBy(f => f.MetadataToken) // Preserve declaration order
                             .ToArray();

            var nextBitIndex = 0;

            foreach (var field in fields)
            {
                if (field.Name == nameof(None))
                    continue;

                var bitIndexAttr = field.GetCustomAttribute<BitIndexAttribute>();

                BigFlagsValue<TMarker> value;

                if (bitIndexAttr != null)
                {
                    value = new BigFlagsValue<TMarker>(bitIndexAttr.Index);
                    field.SetValue(null, value);
                    nextBitIndex = Math.Max(nextBitIndex, bitIndexAttr.Index + 1);
                } else
                {
                    value = new BigFlagsValue<TMarker>(nextBitIndex);
                    field.SetValue(null, value);
                    nextBitIndex++;
                }

                // Cache metadata for fast lookups
                CachedNames.Add(field.Name);
                CachedValues.Add(value);
                CachedNameToValue[field.Name] = value;
                CachedNameToValueIgnoreCase[field.Name] = value;
                CachedValueToName[value] = field.Name;
            }
        }
    }

    /// <summary>
    ///     Checks if a flag name is defined in the derived class.
    /// </summary>
    /// <param name="name">
    ///     The name to check
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <returns>
    ///     True if the name is defined, false otherwise
    /// </returns>
    public static bool IsDefined(string name, bool ignoreCase = false) => TryParse(name, ignoreCase, out _);

    /// <summary>
    ///     Checks if a flag value is defined in the derived class.
    /// </summary>
    public static bool IsDefined(BigFlagsValue<TMarker> value) => GetName(value) != null;

    /// <summary>
    ///     Parses a flag name into its corresponding value.
    /// </summary>
    /// <param name="name">
    ///     The name of the flag to parse
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <returns>
    ///     The parsed flag value
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the name is not a defined flag
    /// </exception>
    public static BigFlagsValue<TMarker> Parse(string name, bool ignoreCase = false)
    {
        if (TryParse(name, ignoreCase, out var value))
            return value;

        throw new ArgumentException($"'{name}' is not a defined flag name in {typeof(TMarker).Name}", nameof(name));
    }

    /// <summary>
    ///     Returns a string representation of the flag value showing all set flag names. If the value matches a single defined
    ///     flag, returns its name. If multiple flags are set, returns a comma-separated list. If no flags are set, returns
    ///     "None".
    /// </summary>
    /// <param name="value">
    ///     The flag value to convert to string
    /// </param>
    /// <returns>
    ///     A string representation of the flag value
    /// </returns>
    public static string ToString(BigFlagsValue<TMarker> value)
    {
        if (value.IsEmpty)
            return "None";

        // First check if it's a single defined flag
        var name = GetName(value);

        if (name != null)
            return name;

        // Otherwise, find all matching individual flags from cached data
        var matchingNames = new List<string>();
        BigInteger combinedValue = 0;

        for (var i = 0; i < CachedValues.Count; i++)
            if (value.HasFlag(CachedValues[i]))
            {
                matchingNames.Add(CachedNames[i]);
                combinedValue |= CachedValues[i].Value;
            }

        // Only return flag names if they EXACTLY represent the value (no undefined bits)
        return (combinedValue == value.Value) && (matchingNames.Count > 0) ? string.Join(", ", matchingNames) : value.Value.ToString();
    }

    /// <summary>
    ///     Tries to parse a flag name into its corresponding value.
    /// </summary>
    /// <param name="name">
    ///     The name of the flag to parse
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <param name="value">
    ///     The parsed flag value, or None if parsing failed
    /// </param>
    /// <returns>
    ///     True if parsing succeeded, false otherwise
    /// </returns>
    public static bool TryParse(string name, bool ignoreCase, out BigFlagsValue<TMarker> value)
    {
        // Handle comma-separated flag values
        if (name.Contains(','))
        {
            var flagNames = name.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var result = None;

            foreach (var flagName in flagNames)
            {
                // Skip "None" as it represents an empty flags value
                if (string.Equals(flagName, "None", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!TryParseSingle(flagName, ignoreCase, out var flag))
                {
                    value = None;

                    return false;
                }

                result |= flag;
            }

            value = result;

            return true;
        }

        return TryParseSingle(name, ignoreCase, out value);
    }

    /// <summary>
    ///     Tries to parse a flag name into its corresponding value (case-sensitive).
    /// </summary>
    public static bool TryParse(string name, out BigFlagsValue<TMarker> value) => TryParse(name, false, out value);

    private static bool TryParseSingle(string name, bool ignoreCase, out BigFlagsValue<TMarker> value)
    {
        var lookup = ignoreCase ? CachedNameToValueIgnoreCase : CachedNameToValue;

        if (lookup.TryGetValue(name, out var result))
        {
            value = result;

            return true;
        }

        value = None;

        return false;
    }
}

/// <summary>
///     Automatic initializer for all BigFlags-derived types
/// </summary>
[SuppressMessage("Usage", "CA2255:The \'ModuleInitializer\' attribute should not be used in libraries")]
internal static class BigFlagsInitializer
{
    /// <summary>
    ///     Module initializer that runs before Main() to ensure all BigFlags types are properly initialized
    /// </summary>
    [ModuleInitializer]
    internal static void Initialize()
    {
        // Initialize all currently loaded assemblies
        InitializeLoadedAssemblies();

        // Hook into assembly load events to catch assemblies loaded after this initializer runs
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    private static void InitializeAssembly(Assembly assembly)
    {
        if (assembly.IsDynamic)
            return;

        try
        {
            var bigFlagsTypes = assembly.GetTypes()
                                        .Where(asmType => asmType is
                                        {
                                            IsInterface: false, IsAbstract: false, BaseType: not null, BaseType.IsGenericType: true
                                        })
                                        .Where(asmType => asmType.BaseType!.GetGenericTypeDefinition() == typeof(BigFlags<>));

            foreach (var asmType in bigFlagsTypes)
                RuntimeHelpers.RunClassConstructor(asmType.TypeHandle);
        } catch
        {
            // Skip assemblies that can't be scanned (e.g., resource assemblies, reflection-only assemblies)
        }
    }

    private static void InitializeLoadedAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
            InitializeAssembly(assembly);
    }

    private static void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs args) => InitializeAssembly(args.LoadedAssembly);
}