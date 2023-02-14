using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class SetElementScript : ConfigurableItemScriptBase
{
    /// <inheritdoc />
    public SetElementScript(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnEquipped(Aisling aisling)
    {
        if (OffenseElement.HasValue)
            aisling.StatSheet.SetOffenseElement(OffenseElement.Value);

        if (DefenseElement.HasValue)
            aisling.StatSheet.SetDefenseElement(DefenseElement.Value);

        if (OffenseElement.HasValue || DefenseElement.HasValue)
            aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }

    /// <inheritdoc />
    public override void OnUnEquipped(Aisling aisling)
    {
        if (OffenseElement.HasValue)
            aisling.StatSheet.SetOffenseElement(Element.None);

        if (DefenseElement.HasValue)
            aisling.StatSheet.SetDefenseElement(Element.None);

        if (OffenseElement.HasValue || DefenseElement.HasValue)
            aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }

    #region ScriptVars
    public Element? OffenseElement { get; init; }
    public Element? DefenseElement { get; init; }
    #endregion
}