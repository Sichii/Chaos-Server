using Chaos.Containers.Abstractions;
using Chaos.Core.Definitions;
using Chaos.PanelObjects;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class Equipment : PanelBase<Item>
{
    public Item? this[EquipmentSlot slot] => this[(byte)slot];
    
    public Equipment(ILogger logger)
        : base(PanelType.Equipment, 19, new byte[] { 0 }, logger) { }
}