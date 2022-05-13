using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class BasicScript : ItemScriptBase
{
    public BasicScript(Item item)
        : base(item) { }

    public override void OnUnequip(User user) { }

    public override void OnUse(User user) => user.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can't use that");
}