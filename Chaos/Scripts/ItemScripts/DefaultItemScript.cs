using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class DefaultItemScript : ItemScriptBase
{
    public DefaultItemScript(Item item)
        : base(item) { }

    public override void OnUse(Aisling source) => source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can't use that");
}