using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using Chaos.Storage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.ItemScripts
{
    public class TownScrollScript : ConfigurableItemScriptBase
    {
        protected Location destination { get; init; }
        private readonly ISimpleCache simpleC;

        public TownScrollScript(Item subject, ISimpleCache simpleCache) : base(subject)
        {
            simpleC = simpleCache;
        }

        public override void OnUse(Aisling source)
        {
            if (source.IsAlive)
            {
                var MapInsance = simpleC.Get<MapInstance>(destination.Map);
                source.TraverseMap(MapInsance, destination);
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            }
        }
    }
}
