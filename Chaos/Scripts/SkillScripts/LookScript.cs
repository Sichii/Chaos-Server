using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SkillScripts
{
    public class LookScript : SkillScriptBase
    {
        public LookScript(Skill subject) : base(subject)
        {
        }

        public override void OnUse(Creature source)
        {
            foreach (var aisling in source.MapInstance.GetEntitiesWithinRange<Aisling>(source))
            {
                if (aisling.Id == source.Id)
                {
                    aisling.Client.SendServerMessage(ServerMessageType.WoodenBoard, "Map Name: " + aisling.MapInstance.Name + " \nHeight: " + aisling.MapInstance.Template.Height + " \nWidth: " + aisling.MapInstance.Template.Width);
                }
            }
        }
    }
}
