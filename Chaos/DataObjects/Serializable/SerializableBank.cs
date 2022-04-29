using System.Collections.Generic;

namespace Chaos.DataObjects.Serializable;

public record SerializableBank
{
    public uint Gold { get; set; }
    public List<SerializableBankItem> Items { get; set; } = new();
}