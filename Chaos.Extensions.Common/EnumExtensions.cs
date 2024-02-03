using Chaos.Common.Definitions;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Enum" />
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    ///     Gets the names of the enum values of the specified type
    /// </summary>
    public static IEnumerable<string> GetEnumNames<T>()
    {
        var type = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(type);

        if (underlyingType is not null)
            type = underlyingType;

        if (!type.IsEnum)
            throw new InvalidOperationException($"{type.Name} is not an enum");

        if (underlyingType is not null)
            return Enum.GetNames(type)
                       .Prepend(string.Empty);

        return Enum.GetNames(type);
    }

    /// <summary>
    ///     Gets the individual flag parts of a flag enum value />
    /// </summary>
    /// <param name="input">
    ///     An enum value with one or more flags
    /// </param>
    /// <typeparam name="T">
    ///     An enum type
    /// </typeparam>
    public static IEnumerable<T> GetFlags<T>(this T input) where T: Enum
    {
        foreach (T value in Enum.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }

    /// <summary>
    ///     Converts an <see cref="EquipmentType" /> to one or more <see cref="EquipmentSlot" />s
    /// </summary>
    /// <param name="type">
    ///     An <see cref="EquipmentType" /> value
    /// </param>
    /// <returns>
    ///     One or more <see cref="EquipmentSlot" />s associated with the given <see cref="EquipmentType" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the given <see cref="EquipmentType" /> is not defined
    /// </exception>
    public static IEnumerable<EquipmentSlot> ToEquipmentSlots(this EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.NotEquipment:
                yield return EquipmentSlot.None;

                yield break;
            case EquipmentType.Weapon:
                yield return EquipmentSlot.Weapon;

                yield break;
            case EquipmentType.Armor:
                yield return EquipmentSlot.Armor;

                yield break;
            case EquipmentType.OverArmor:
                yield return EquipmentSlot.Overcoat;

                yield break;
            case EquipmentType.Shield:
                yield return EquipmentSlot.Shield;

                yield break;
            case EquipmentType.Helmet:
                yield return EquipmentSlot.Helmet;

                yield break;
            case EquipmentType.OverHelmet:
                yield return EquipmentSlot.OverHelm;

                yield break;
            case EquipmentType.Earrings:
                yield return EquipmentSlot.Earrings;

                yield break;
            case EquipmentType.Necklace:
                yield return EquipmentSlot.Necklace;

                yield break;
            case EquipmentType.Ring:
                yield return EquipmentSlot.LeftRing;
                yield return EquipmentSlot.RightRing;

                yield break;
            case EquipmentType.Gauntlet:
                yield return EquipmentSlot.LeftGaunt;
                yield return EquipmentSlot.RightGaunt;

                yield break;
            case EquipmentType.Belt:
                yield return EquipmentSlot.Belt;

                yield break;
            case EquipmentType.Greaves:
                yield return EquipmentSlot.Greaves;

                yield break;
            case EquipmentType.Boots:
                yield return EquipmentSlot.Boots;

                yield break;
            case EquipmentType.Accessory:
                yield return EquipmentSlot.Accessory1;
                yield return EquipmentSlot.Accessory2;
                yield return EquipmentSlot.Accessory3;

                yield break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}