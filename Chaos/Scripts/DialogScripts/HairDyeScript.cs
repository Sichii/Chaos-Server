using Chaos.Common.Definitions;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class HairDyeScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private DisplayColor? displayColor;
        private Item? item;

        public HairDyeScript(Dialog subject) : base(subject)
        {

        }
    }
}
