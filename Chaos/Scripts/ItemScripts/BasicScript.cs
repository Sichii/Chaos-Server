using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class BasicScript : ItemScriptBase
{
    public BasicScript(Item item)
        : base(item) { }

    public override void OnUnequip(Aisling aisling) { }

    public override void OnUse(Aisling aisling) => aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can't use that");
}