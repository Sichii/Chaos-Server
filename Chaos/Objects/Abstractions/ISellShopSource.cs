using Chaos.Objects.Panel;

namespace Chaos.Objects.Abstractions;

public interface ISellShopSource : IDialogSourceEntity
{
    ICollection<string> ItemsToBuy { get; }

    bool IsBuying(Item item);
}