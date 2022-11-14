using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Factories;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class HairstyleScript : DialogScriptBase
    {

        public int Hairstyle;
        private Item? hairItem;
        private readonly IItemFactory ItemFactory;
        private static List<string> femaleHairstyles = new List<string> { "female_hairstyle_1", "female_hairstyle_2", "female_hairstyle_3", "female_hairstyle_4"
        , "female_hairstyle_5", "female_hairstyle_6", "female_hairstyle_7", "female_hairstyle_8", "female_hairstyle_9", "female_hairstyle_10", "female_hairstyle_11",
         "female_hairstyle_12", "female_hairstyle_13", "female_hairstyle_14", "female_hairstyle_15", "female_hairstyle_16", "female_hairstyle_17",
         "female_hairstyle_19", "female_hairstyle_20", "female_hairstyle_21", "female_hairstyle_22", "female_hairstyle_23", "female_hairstyle_24", "female_hairstyle_25",
         "female_hairstyle_26", "female_hairstyle_27", "female_hairstyle_28", "female_hairstyle_29", "female_hairstyle_30", "female_hairstyle_31", "female_hairstyle_32",
         "female_hairstyle_33", "female_hairstyle_34", "female_hairstyle_35", "female_hairstyle_36", "female_hairstyle_37", "female_hairstyle_38", "female_hairstyle_39",
         "female_hairstyle_40", "female_hairstyle_41", "female_hairstyle_42", "female_hairstyle_43", "female_hairstyle_44", "female_hairstyle_45", "female_hairstyle_46",
         "female_hairstyle_47", "female_hairstyle_48", "female_hairstyle_49", "female_hairstyle_50", "female_hairstyle_51", "female_hairstyle_52", "female_hairstyle_53",
         "female_hairstyle_54", "female_hairstyle_55", "female_hairstyle_56", "female_hairstyle_57", "female_hairstyle_58", "female_hairstyle_59", "female_hairstyle_60"
        };
        private static List<string> maleHairstyles = new List<string> { "male_hairstyle_1", "male_hairstyle_2", "male_hairstyle_3", "male_hairstyle_4"
        , "male_hairstyle_5", "male_hairstyle_6", "male_hairstyle_7", "male_hairstyle_8", "male_hairstyle_9", "male_hairstyle_10", "male_hairstyle_11",
         "male_hairstyle_12", "male_hairstyle_13", "male_hairstyle_14", "male_hairstyle_15", "male_hairstyle_16", "male_hairstyle_17",
         "male_hairstyle_19", "male_hairstyle_20", "male_hairstyle_21", "male_hairstyle_22", "male_hairstyle_23", "male_hairstyle_24", "male_hairstyle_25",
         "male_hairstyle_26", "male_hairstyle_27", "male_hairstyle_28", "male_hairstyle_29", "male_hairstyle_30", "male_hairstyle_31", "male_hairstyle_32",
         "male_hairstyle_33", "male_hairstyle_34", "male_hairstyle_35", "male_hairstyle_36", "male_hairstyle_37", "male_hairstyle_38", "male_hairstyle_39",
         "male_hairstyle_40", "male_hairstyle_41", "male_hairstyle_42", "male_hairstyle_43", "male_hairstyle_44", "male_hairstyle_45", "male_hairstyle_46",
         "male_hairstyle_47", "male_hairstyle_48", "male_hairstyle_49", "male_hairstyle_50", "male_hairstyle_51", "male_hairstyle_52", "male_hairstyle_53",
         "male_hairstyle_54", "male_hairstyle_55", "male_hairstyle_56", "male_hairstyle_57", "male_hairstyle_58", "male_hairstyle_59", "male_hairstyle_60"
        };

        public HairstyleScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
        }

        public override void OnDisplaying(Aisling source)
        {

            if (source.Gender.Equals(Gender.Male))
            {
                foreach (var s in maleHairstyles)
                {
                    var item = ItemFactory.CreateFaux(s);
                    item.Color = source.HairColor;
                    Subject.Items.Add(item);
                }
            }
            if (source.Gender.Equals(Gender.Female))
            {
                foreach (var s in femaleHairstyles)
                {
                    var item = ItemFactory.CreateFaux(s);
                    item.Color = source.HairColor;
                    Subject.Items.Add(item);
                }
            }
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var hairStyleName))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }
            var Item = Subject.Items.FirstOrDefault(x => x.DisplayName.EqualsI(hairStyleName));
            if (Item == null)
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }
            if (!source.TryTakeGold(Item.Template.BuyCost))
            {
                Subject.Close(source);
                return;
            }
            source.HairStyle = Item.Template.ItemSprite.DisplaySprite;
            source.Refresh(true);
        }
    }
}
