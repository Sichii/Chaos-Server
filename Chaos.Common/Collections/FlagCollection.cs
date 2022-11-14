using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Common.Collections;

[JsonConverter(typeof(FlagCollectionConverter))]
public sealed class FlagCollection : IEnumerable<KeyValuePair<Type, Enum>>
{
    private readonly ConcurrentDictionary<Type, Enum> Flags;

    public FlagCollection() => Flags = new ConcurrentDictionary<Type, Enum>();

    public void AddFlag<T>(T flag) where T: Enum => AddFlag(typeof(T), flag);

    public void AddFlag(Type flagType, Enum flagValue)
    {
        if (Flags.ContainsKey(flagType))
            Flags[flagType] = (Enum)Enum.ToObject(flagType, Convert.ToUInt64(Flags[flagType]) | Convert.ToUInt64(flagValue));
        else
            Flags.TryAdd(flagType, flagValue);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, Enum>> GetEnumerator() => Flags.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ulong GetFlag<T>() where T: Enum
    {
        var flagType = typeof(T);

        if (Flags.ContainsKey(flagType))
            return Convert.ToUInt64(Flags[flagType]);

        return 0;
    }

    public bool HasFlag<T>(T flag) where T: Enum
    {
        var flagType = typeof(T);

        if (Flags.ContainsKey(flagType))
            return (Convert.ToUInt64(Flags[flagType]) & Convert.ToUInt64(flag)) != 0;

        return false;
    }

    public void RemoveFlag<T>(T flag) where T: Enum => RemoveFlag(typeof(T), flag);

    public void RemoveFlag(Type flagType, Enum flagValue)
    {
        if (Flags.ContainsKey(flagType))
            Flags[flagType] = (Enum)Enum.ToObject(flagType, Convert.ToUInt64(Flags[flagType]) & ~Convert.ToUInt64(flagValue));
    }
}