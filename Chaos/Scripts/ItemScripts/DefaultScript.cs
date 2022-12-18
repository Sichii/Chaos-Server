using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class DefaultScript : ItemScriptBase
{
    public DefaultScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source) => source.SendOrangeBarMessage("You can't use that");
}