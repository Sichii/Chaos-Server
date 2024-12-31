#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel.Abstractions;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     A panel that contains skills or spells
/// </summary>
/// <typeparam name="T">
///     Skill or Spell
/// </typeparam>
public abstract class KnowledgeBookBase<T> : PanelBase<T>, IKnowledgeBook<T> where T: PanelEntityBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KnowledgeBookBase{T}" /> class
    /// </summary>
    /// <param name="panelType">
    ///     The type of panel this is
    /// </param>
    /// <param name="abilities">
    ///     The abilities to populate the knowledge book with
    /// </param>
    protected KnowledgeBookBase(PanelType panelType, IEnumerable<T>? abilities = null)
        : base(
            panelType,
            89 + GetPanelTypeAdjustment(panelType),
            [
                0,
                36,
                72
            ])
    {
        abilities ??= Array.Empty<T>();

        foreach (var ability in abilities)
            Objects[ability.Slot] = ability;
    }

    // <inheritdoc />
    public byte GetFirstSlotInPage(PageType page)
        => page switch
        {
            PageType.Page1 => 0,
            PageType.Page2 => 37,
            PageType.Page3 => 73,
            _              => throw new ArgumentOutOfRangeException(nameof(page), page, null)
        };

    // <inheritdoc />
    public byte GetLastSlotInPage(PageType page)
        => page switch
        {
            PageType.Page1 => 35,
            PageType.Page2 => 71,
            PageType.Page3 => (byte)(Length - 1),
            _              => throw new ArgumentOutOfRangeException(nameof(page), page, null)
        };

    /// <inheritdoc />
    public bool TryAddToNextSlot(PageType page, T obj)
    {
        if (IsFull)
            return false;

        using var @lock = Sync.EnterScope();

        var firstPossibleSlot = GetFirstSlotInPage(page);
        var lastPossibleSlot = GetLastSlotInPage(page);

        for (var i = firstPossibleSlot; i <= lastPossibleSlot; i++)
            if ((Objects[i] == null) && IsValidSlot(i))
                return TryAdd(i, obj);

        return TryAddToNextSlot(obj);
    }

    private static byte GetPanelTypeAdjustment(PanelType panelType)
        => panelType switch
        {
            PanelType.SkillBook => 0,
            PanelType.SpellBook => 1,
            _                   => throw new ArgumentOutOfRangeException(nameof(panelType), panelType, null)
        };
}