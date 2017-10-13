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
    internal delegate void OnUseDelegate(Client client, Server server, object args);
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
        #endregion

        #region Items
        private void NormalObj(Client client, Server server, object args) { }
        private void Equipment(Client client, Server server, object args)
        {
            Item item = (Item)args;
            Item outItem;
            if (client.User.Equipment.Contains(item))
            {
                if (client.User.Equipment.TryUnequip(item.EquipmentPair.Item1, out outItem))
                {
                    client.Enqueue(server.Packets.RemoveEquipment(item.EquipmentPair.Item1));
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

            foreach (User user in Game.World.ObjectsVisibleFrom(client.User).OfType<User>())
                user.Client.Enqueue(server.Packets.DisplayUser(client.User));

            client.Enqueue(server.Packets.DisplayUser(client.User));
        }
        private bool Exchange(Client client, Server server, Item item)
        {
            //if client is in an active exchange
            //place the item in the exchange

            return false;
        }

        private Item AdminTrinket(int count) => new Item(0, 13709, "Admin Trinket", count, TimeSpan.Zero, null, true);
        private void AdminTrinket(Client client, Server server, object args)
        {
            client.ActiveObject = args;
            client.CurrentDialog = Game.Dialogs[1];
            client.Enqueue(server.Packets.DisplayDialog(args, client.CurrentDialog));
        }

        private Item TestItem(int count) => new Item(0, 1108, "Test Item", count, TimeSpan.Zero, null, false, 0, true);
        private Item TestEquipment(int count) => new Item(0, 1108, "Test Equipment", count, TimeSpan.Zero, new Tuple<EquipmentSlot, ushort>(EquipmentSlot.Armor, 44), false, 0, false, 10000, 10000, 5);
        #endregion

        #region Skills
        private Skill TestSkill1() => new Skill(0, "Test Skill 1", 78, TimeSpan.Zero);
        private void TestSkill1(Client client, Server server, object args)
        {
            //do things
        }
        #endregion

        #region Spells
        private Spell Mend() => new Spell(0, "Mend", SpellType.Targeted, 118, string.Empty, 1, TimeSpan.Zero);
        private void Mend(Client client, Server server, object args)
        {
            Creature target = (Creature)args;
            Animation animation = new Animation(target.Id, client.User.Id, 4, 0, 100);
            uint baseAmount = 10;

            baseAmount += (uint)(client.User.Attributes.CurrentWis * 5);
            target.CurrentHP = (target.CurrentHP + baseAmount) > target.MaximumHP ? target.MaximumHP : (target.CurrentHP + baseAmount);

            foreach (User nearbyUser in Game.World.ObjectsVisibleFrom(target).OfType<User>())
            {
                nearbyUser.Client.Enqueue(server.Packets.Animation(animation));
                nearbyUser.Client.Enqueue(server.Packets.HealthBar(target));
            }

            User user = target as User;

            user?.Client.Enqueue(server.Packets.Animation(animation));
            user?.Client.Enqueue(server.Packets.HealthBar(target));
            user?.Client.SendAttributes(StatUpdateFlags.Vitality);
        }

        private Spell Heal() => new Spell(0, "Heal", SpellType.Targeted, 21, string.Empty, 1, new TimeSpan(0, 0, 2));
        private void Heal(Client client, Server server, object args)
        {
            Creature target = (Creature)args;
            Animation animation = new Animation(target.Id, client.User.Id, 157, 0, 100);
            uint baseAmount = 100000;

            baseAmount += (uint)(client.User.Attributes.CurrentWis * 500);
            target.CurrentHP = (target.CurrentHP + baseAmount) > target.MaximumHP ? target.MaximumHP : (target.CurrentHP + baseAmount);

            foreach (User nearbyUser in Game.World.ObjectsVisibleFrom(target).OfType<User>())
            {
                nearbyUser.Client.Enqueue(server.Packets.Animation(animation));
                nearbyUser.Client.Enqueue(server.Packets.HealthBar(target));
            }

            User user = target as User;

            user?.Client.Enqueue(server.Packets.Animation(animation));
            user?.Client.Enqueue(server.Packets.HealthBar(target));
            user?.Client.SendAttributes(StatUpdateFlags.Vitality);
        }
        private Spell SradTut() => new Spell(0, "Srad Tut", SpellType.Targeted, 21, string.Empty, 1, new TimeSpan(0, 0, 2));
        private void SradTut(Client client, Server server, object args)
        {
            Creature target = (Creature)args;
            Animation animation = new Animation(target.Id, client.User.Id, 158, 0, 100);
            uint baseAmount = 100000;

            baseAmount += (uint)(client.User.Attributes.CurrentWis * 500);
            target.CurrentHP = (target.CurrentHP - baseAmount) > target.MaximumHP ? target.MaximumHP : (target.CurrentHP - baseAmount);
            

            foreach (User nearbyUser in Game.World.ObjectsVisibleFrom(target).OfType<User>())
            {
                nearbyUser.Client.Enqueue(server.Packets.Animation(animation));
                nearbyUser.Client.Enqueue(server.Packets.HealthBar(target));
            }

            User user = target as User;

            user?.Client.Enqueue(server.Packets.Animation(animation));
            user?.Client.Enqueue(server.Packets.HealthBar(target));
            user?.Client.SendAttributes(StatUpdateFlags.Vitality);
        }
        #endregion
    }
}
