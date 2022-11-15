using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Data;
using Chaos.Formulae;
using Chaos.Objects;
using static System.Net.Mime.MediaTypeNames;
using Chaos.Storage.Abstractions;
using Chaos.Containers;

namespace Chaos.Scripts.SpellScripts
{
    public class HomeScript : ConfigurableSpellScriptBase
    {

        protected Location destination { get; init; }
        private readonly ISimpleCache simpleC;


        public HomeScript(Spell subject, ISimpleCache simpleCache) : base(subject)
        {
            simpleC = simpleCache;
        }

        public override void OnUse(SpellContext context)
        {
            //Lots to unpack here. This could use the players legend to grab their home, randomly put them on any of the World's Inn Maps, could be a guild house or whatever. For now, it functions like the scrolls.

            var source = context.Source;
            if (source.IsAlive)
            {
                var MapInsance = simpleC.Get<MapInstance>(destination.Map);
                source.TraverseMap(MapInsance, destination);
            }
        }
    }
}
