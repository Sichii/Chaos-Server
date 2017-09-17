using System;
using System.Collections.Generic;

namespace Chaos
{
    internal delegate void OnUseDelegate(Client client, Server server, Item item);
    internal delegate Item ItemCreationDelegate(int count);
    internal delegate Skill SkillCreationDelegate();
    internal delegate Spell SpellCreationDelegate();
    internal class CreationEngine
    {

        private Dictionary<string, ItemCreationDelegate> Items { get; }
        private Dictionary<string, SkillCreationDelegate> Skills { get; }
        private Dictionary<string, SpellCreationDelegate> Spells { get; }
        private Dictionary<string, OnUseDelegate> Effects { get; }

        internal CreationEngine()
        {
            Items = new Dictionary<string, ItemCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Skills = new Dictionary<string, SkillCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Spells = new Dictionary<string, SpellCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Effects = new Dictionary<string, OnUseDelegate>(StringComparer.CurrentCultureIgnoreCase);

            #region Items
            Items.Add("Admin Trinket", new ItemCreationDelegate(AdminTrinket));
            Items.Add("Test Item", new ItemCreationDelegate(TestItem));
            #endregion


            #region Skills
            Skills.Add("Test Skill 1", new SkillCreationDelegate(TestSkill1));
            #endregion


            #region Spells
            Spells.Add("Test Spell 1", new SpellCreationDelegate(TestSpell1));
            #endregion


            #region OnUseDelegates
            Effects.Add("NormalObj", new OnUseDelegate(NormalObj));
            Effects.Add("Admin Trinket", new OnUseDelegate(AdminTrinket));
            Effects.Add("Test Item", new OnUseDelegate(NormalObj));
            Effects.Add("Test Equipment", new OnUseDelegate(Equipment));
            Effects.Add("Test Skill 1", new OnUseDelegate(TestSkill1));
            Effects.Add("Test Spell 1", new OnUseDelegate(TestSpell1));
            #endregion
        }
        internal OnUseDelegate GetEffect(string itemName) => Effects.ContainsKey(itemName) ? Effects[itemName] : Effects["NormalObj"];
        internal Gold CreateGold(Client client, uint amount, Point groundPoint) => new Gold(GetGoldSprite(amount), groundPoint, client.User.Map, amount);
        internal Item CreateItem(string name) => Items[name](1);
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
            if (amount >= 5000)
                return 140;
            else if (amount >= 1000)
                return 141;
            else if (amount >= 500)
                return 142;
            else if (amount >= 100)
                return 137;
            else if (amount > 1)
                return 138;
            else
                return 139;
        }
        internal Skill CreateSkill(string name) => Skills[name]();
        internal Spell CreateSpell(string name) => Spells[name]();

        #region Items
        private Item AdminTrinket(int count) => new Item(0, 13709, "Admin Trinket", count, TimeSpan.Zero, EquipmentSlot.None, true);
        private Item TestItem(int count) => new Item(0, 1108, "Test Item", count, TimeSpan.Zero, EquipmentSlot.None, false, 0, true);
        private Item TestEquipment(int count) => new Item(0, 1108, "Test Equipment", count, TimeSpan.Zero, EquipmentSlot.Armor, false, 0, true, 10000, 10000, 5);
        #endregion
        #region Skills
        private Skill TestSkill1() => new Skill(0, "Test Skill 1", 78, TimeSpan.Zero);
        #endregion
        #region Spells
        private Spell TestSpell1() => new Spell(0, "Test Spell 1", 1, 118, string.Empty, 2, TimeSpan.Zero);
        #endregion


        #region CommonDelegates
        private bool Exchange(Client client, Server server, Item item)
        {
            //if client is in an active exchange
            //place the item in the exchange

            return false;
        }
        private void NormalObj(Client client, Server server, Item item) { }
        private void Equipment(Client client, Server server, Item item)
        {
            Item outItem;
            if (client.User.Equipment.Contains(item))
            {
                if (client.User.Equipment.TryUnequip(item.EquipmentSlot, out outItem))
                {
                    client.Enqueue(server.Packets.RemoveEquipment(item.EquipmentSlot));
                    if (client.User.Inventory.AddToNextSlot(outItem))
                        client.Enqueue(server.Packets.AddItem(item));
                }
            }
            else if (client.User.Equipment.TryEquip(item, out outItem))
            {
                client.Enqueue(server.Packets.RemoveItem(item.Slot));
                client.Enqueue(server.Packets.AddEquipment(item));

                if (outItem != null && client.User.Inventory.AddToNextSlot(outItem))
                    client.Enqueue(server.Packets.AddItem(outItem));
            }
        }
        #endregion

        #region OnUseDelegates
        private void AdminTrinket(Client client, Server server, Item item)
        {
            client.ActiveObject = item;
            client.CurrentDialog = Game.Dialogs[1];
            client.Enqueue(server.Packets.DisplayDialog(item, client.CurrentDialog));
        }
        private void TestSkill1(Client client, Server server, Item item)
        {
            //do things
        }
        private void TestSpell1(Client client, Server server, Item item)
        {
            //do things
        }
        #endregion
    }
}
