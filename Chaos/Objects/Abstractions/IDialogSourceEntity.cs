using Chaos.Common.Definitions;

namespace Chaos.Objects.Abstractions;

public interface IDialogSourceEntity
{
    DisplayColor Color { get; }
    EntityType EntityType { get; }
    uint Id { get; }
    string Name { get; }
    ushort Sprite { get; }
}