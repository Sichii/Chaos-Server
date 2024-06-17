using Chaos.Common.Definitions;
using Chaos.Models.Panel.Abstractions;

namespace Chaos.Collections.Abstractions;

public abstract class KnowledgeBookBase<T> : PanelBase<T>, IKnowledgeBook<T> where T: PanelEntityBase
{
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

    public byte GetFirstSlotInPage(PageType page)
        => page switch
        {
            PageType.Page1 => 0,
            PageType.Page2 => 37,
            PageType.Page3 => 73,
            _              => throw new ArgumentOutOfRangeException(nameof(page), page, null)
        };

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

        using var @lock = Sync.Enter();

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