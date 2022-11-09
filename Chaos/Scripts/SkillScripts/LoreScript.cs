using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.SkillScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SkillScripts
{
    public class LoreScript : SkillScriptBase
    {
        public LoreScript(Skill subject) : base(subject)
        {
        }

        public override void OnUse(Creature source)
        {
            foreach (var aisling in source.MapInstance.GetEntitiesWithinRange<Aisling>(source))
            {
                if (aisling.Id == source.Id)
                {
                    aisling.Client.SendServerMessage(ServerMessageType.WoodenBoard, "Name: " + aisling.Inventory[1]?.DisplayName + "\nValue: " + aisling.Inventory[1]?.Template.SellValue + "\nArmor Class: " + aisling.Inventory[1]?.Template?.Modifiers?.Ac);
                }
            }
        }
    }
}
