// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal delegate void OnUseDelegate(Client client, Server server, params object[] args);
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
            #region Items
            Items = new Dictionary<string, ItemCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Items.Add("Admin Trinket", new ItemCreationDelegate(AdminTrinket));
            Items.Add("Test Item", new ItemCreationDelegate(TestItem));
            Items.Add("Test Equipment", new ItemCreationDelegate(TestEquipment));
            #endregion

            #region Skills
            Skills = new Dictionary<string, SkillCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Skills.Add("Test Skill 1", new SkillCreationDelegate(TestSkill1));
            #endregion

            #region Spells
            Spells = new Dictionary<string, SpellCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Spells.Add("Mend", new SpellCreationDelegate(Mend));
            Spells.Add("Heal", new SpellCreationDelegate(Heal));
            Spells.Add("Srad Tut", new SpellCreationDelegate(SradTut));
            #endregion

            #region OnUseDelegates
            Effects = new Dictionary<string, OnUseDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Effects.Add("NormalObj", new OnUseDelegate(NormalObj));
            Effects.Add("Admin Trinket", new OnUseDelegate(AdminTrinket));
            Effects.Add("Test Item", new OnUseDelegate(NormalObj));
            Effects.Add("Test Equipment", new OnUseDelegate(Equipment));
            Effects.Add("Test Skill 1", new OnUseDelegate(TestSkill1));
            Effects.Add("Mend", new OnUseDelegate(Mend));
            Effects.Add("Heal", new OnUseDelegate(Heal));
            Effects.Add("Srad Tut", new OnUseDelegate(SradTut));
            #endregion
        }

        #region Interface
        internal OnUseDelegate GetEffect(string itemName) => Effects.ContainsKey(itemName) ? Effects[itemName] : Effects["NormalObj"];
        internal Gold CreateGold(Client client, uint amount, Point groundPoint) => new Gold(GetGoldSprite(amount), groundPoint, client.User.Map, amount);
        internal Item CreateItem(string name) => Items.ContainsKey(name) ? Items[name](1) : null;
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
        internal Skill CreateSkill(string name) => Skills.ContainsKey(name) ? Skills[name]() : null;
        internal Spell CreateSpell(string name) => Spells.ContainsKey(name) ? Spells[name]() : null;
        #endregion

        #region Items
        private void NormalObj(Client client, Server server, params object[] args) { }
        private void Equipment(Client client, Server server, params object[] args)
        {
            Item item = (Item)args[0];
            Item outItem;
            if (client.User.Equipment.Contains(item))
            {
                if (client.User.Equipment.TryUnequip(item.EquipmentPair.Item1, out outItem))
                {
                    client.Enqueue(ServerPackets.RemoveEquipment(item.EquipmentPair.Item1));
                    if (client.User.Inventory.AddToNextSlot(outItem))
                        client.Enqueue(ServerPackets.AddItem(item));
                }
            }
            else if (client.User.Equipment.TryEquip(item, out outItem))
            {
                client.Enqueue(ServerPackets.RemoveItem(item.Slot));
                client.Enqueue(ServerPackets.AddEquipment(item));

                if (outItem != null && client.User.Inventory.AddToNextSlot(outItem))
                    client.Enqueue(ServerPackets.AddItem(outItem));
            }

            foreach (User user in Game.World.ObjectsVisibleFrom(client.User).OfType<User>())
                user.Client.Enqueue(ServerPackets.DisplayUser(client.User));

            client.Enqueue(ServerPackets.DisplayUser(client.User));
        }
        private bool Exchange(Client client, Server server, Item item)
        {
            //if client is in an active exchange
            //place the item in the exchange

            return false;
        }

        private Item AdminTrinket(int count) => new Item(0, 13709, "Admin Trinket", count, TimeSpan.Zero, null, true);
        private void AdminTrinket(Client client, Server server, params object[] args)
        {
            client.ActiveObject = args[0];
            client.CurrentDialog = Game.Dialogs[1];
            client.Enqueue(ServerPackets.DisplayDialog(args[0], client.CurrentDialog));
        }

        private Item TestItem(int count) => new Item(0, 1108, "Test Item", count, TimeSpan.Zero, null, false, 0, true);
        private Item TestEquipment(int count) => new Item(0, 1108, "Test Equipment", count, TimeSpan.Zero, new Tuple<EquipmentSlot, ushort>(EquipmentSlot.Armor, 44), false, 0, false, 10000, 10000, 5, new Animation());
        #endregion

        #region Skills
        private Skill TestSkill1() => new Skill(0, 78, "Test Skill 1", SkillType.Front, TimeSpan.Zero, true, new Animation(), 1);
        private void TestSkill1(Client client, Server server, params object[] args)
        {
            Skill skill = args[0] as Skill;
            Creature target;

            Game.World.TryGetObject(client.User.Point.Offsetter(client.User.Direction), out target, client.User.Map);

            int amount = -50000;
            amount -= (client.User.Attributes.CurrentStr * 250);

            Game.Extensions.ApplySkill(client, amount, skill);
        }
        #endregion

        #region Spells
        private Spell Mend() => new Spell(0, 118, "Mend", SpellType.Targeted, string.Empty, 1, TimeSpan.Zero, new Animation(4, 0, 100), 6);
        private void Mend(Client client, Server server, params object[] args)
        {
            Spell spell = args[0] as Spell;
            Creature target = args[1] as Creature;
            int amount = 10;
            amount += client.User.Attributes.CurrentWis * 5;

            Game.Extensions.ApplySpell(client, amount, spell, target);
        }

        private Spell Heal() => new Spell(0, 21, "Heal", SpellType.Targeted, string.Empty, 1, new TimeSpan(0, 0, 2), new Animation(157, 0, 100), 6);
        private void Heal(Client client, Server server, params object[] args)
        {
            Spell spell = args[0] as Spell;
            Creature target = args[1] as Creature;

            Animation animation = new Animation(spell.EffectAnimation, target.Id, client.User.Id);
            int amount = 100000;
            amount += client.User.Attributes.CurrentWis * 500;

            Game.Extensions.ApplySpell(client, amount, spell, target);
        }
        private Spell SradTut() => new Spell(0, 21, "Srad Tut", SpellType.Targeted, string.Empty, 1, new TimeSpan(0, 0, 2), new Animation(158, 0, 100), 6);
        private void SradTut(Client client, Server server, params object[] args)
        {
            Spell spell = args[0] as Spell;
            Creature target = args[1] as Creature;

            Animation animation = new Animation(spell.EffectAnimation, target.Id, client.User.Id);
            int amount = -100000;
            amount -= client.User.Attributes.CurrentWis * 500;

            Game.Extensions.ApplySpell(client, amount, spell, target);
        }
        #endregion
    }
}
