using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel.Abstractions;

namespace Chaos.Collections.Abstractions;

public interface IKnowledgeBook<T> : IPanel<T> where T: PanelEntityBase
{
    byte GetFirstSlotInPage(PageType page);

    byte GetLastSlotInPage(PageType page);
    bool TryAddToNextSlot(PageType page, T obj);
}