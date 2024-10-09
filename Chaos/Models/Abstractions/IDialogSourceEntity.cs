using Chaos.DarkAges.Definitions;
using Chaos.Models.World;

namespace Chaos.Models.Abstractions;

public interface IDialogSourceEntity
{
    DisplayColor Color { get; }
    EntityType EntityType { get; }
    uint Id { get; }
    string Name { get; }
    ushort Sprite { get; }

    void Activate(Aisling source);
}