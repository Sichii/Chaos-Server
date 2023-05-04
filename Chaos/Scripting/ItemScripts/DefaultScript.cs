using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class DefaultScript : ItemScriptBase
{
    public DefaultScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source) => source.SendOrangeBarMessage("You can't use that");
}