using Chaos.Models.Panel;

namespace Chaos.Models.Abstractions;

public interface ISellShopSource : IDialogSourceEntity
{
    ICollection<string> ItemsToBuy { get; }

    bool IsBuying(Item item);
}