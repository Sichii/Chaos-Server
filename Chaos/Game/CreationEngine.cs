using System;
using System.Collections.Generic;

namespace Chaos
{
    internal delegate void OnUseDelegate(Client client, Server server);
    internal class CreationEngine
    {
        private delegate Item ItemCreationDelegate(int count);
        private delegate Skill SkillCreationDelegate();
        private delegate Spell SpellCreationDelegate();
        private Dictionary<string, ItemCreationDelegate> Items { get; }
        private Dictionary<string, SkillCreationDelegate> Skills { get; }
        private Dictionary<string, SpellCreationDelegate> Spells { get; }
        private Dictionary<string, OnUseDelegate> Effects { get; }

        internal CreationEngine()
        {
            Items = new Dictionary<string, ItemCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);

            #region Items
            Items.Add("Admin Trinket", new ItemCreationDelegate(AdminTrinket));
            Items.Add("Test Equipment", new ItemCreationDelegate(AdminTrinket));
            #endregion


            #region Skills
            Skills.Add("Test Skill 1", new SkillCreationDelegate(TestSkill1));
            #endregion


            #region Spells
            Spells.Add("Test Spell 1", new SpellCreationDelegate(TestSpell1));
            #endregion


            #region OnUseDelegates
            Effects.Add("Admin Trinket", new OnUseDelegate(AdminTrinket));
            Effects.Add("Test Skill 1", new OnUseDelegate(TestSkill1));
            Effects.Add("Test Spell 1", new OnUseDelegate(TestSpell1));
            Effects.Add("Test Equipment", new OnUseDelegate(TestEquipment));
            #endregion
        }

        internal Gold CreateGold(Client client, uint amount, Point groundPoint) => new Gold(GetGoldSprite(amount), groundPoint, client.User.Map, amount);
        internal Item CreateItem(string name)
        {
            Item item = Items[name](1);
            Effects.TryGetValue(name, out item.Activate);

            return item;
        }
        internal Item CreateStackableItem(string name, int count)
        {
            Item item = CreateItem(name);

            if (!item.Stackable)
                item.Count = 1;
            else
                item.Count = count;

            return item;
        }
        internal List<Item> CreateManyItems(string name, int count)
        {
            List<Item> items = new List<Item>();

            for (int i = 0; i < count; i++)
                items.Add(CreateItem(name));

            return items;
        }
        private byte GetGoldSprite(uint amount)
        {
            if (amount > 5000)
                return 140;
            else if (amount > 1000)
                return 141;
            else if (amount > 500)
                return 142;
            else if (amount > 100)
                return 137;
            else if (amount > 1)
                return 138;
            else
                return 139;
        }
        internal Skill CreateSkill(string name)
        {
            Skill skill = Skills[name]();
            Effects.TryGetValue(name, out skill.Activate);

            return skill;
        }
        internal Spell CreateSpell(string name)
        {
            Spell spell = Spells[name]();
            Effects.TryGetValue(name, out spell.Activate);

            return spell;
        }

        #region Items
        private Item AdminTrinket(int count) => new Item(0, 13709, "Admin Trinket", count, TimeSpan.Zero);
        private Item TestEquipment(int count) => new Item(0, 1108, "Test Equipment", count, TimeSpan.Zero);
        #endregion


        #region Skills
        private Skill TestSkill1() => new Skill(0, "Test Skill 1", 78, TimeSpan.Zero);
        #endregion


        #region Spells
        private Spell TestSpell1() => new Spell(0, "Test Spell 1", 1, 118, string.Empty, 2, TimeSpan.Zero);
        #endregion


        #region OnUseDelegates
        private void AdminTrinket(Client client, Server server)
        {
            //do things
        }
        private void TestSkill1(Client client, Server server)
        {
            //do things
        }
        private void TestSpell1(Client client, Server server)
        {
            //do things
        }
        private void TestEquipment(Client client, Server server)
        {

        }
        #endregion
    }
}
