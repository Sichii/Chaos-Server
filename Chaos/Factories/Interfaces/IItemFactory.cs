using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;

namespace Chaos.Factories.Interfaces;

public interface IItemFactory
{
    Item CreateItem(string templateKey, ICollection<string>? extraScriptKeys = null);
    Item DeserializeItem(SerializableItem serializableItem);
    Item CloneItem(Item item);
}