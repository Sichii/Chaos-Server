using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Templates.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts.Tutorial
{
    public class AnnaGiveArmorScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;

        public AnnaGiveArmorScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.Gender == Common.Definitions.Gender.Female)
            {
                if (source.Inventory.Any(Item => Item.Template.TemplateKey.EqualsI("blouse")))
                    return;
                source.TryGiveItems(ItemFactory.Create("blouse"));
            }
            else
            {
                if (source.Inventory.Any(Item => Item.Template.TemplateKey.EqualsI("shirt1")))
                    return;
                source.TryGiveItems(ItemFactory.Create("shirt1"));
            }
        }
    }
}
