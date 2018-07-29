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
    internal delegate void OnUseDelegate(Client client, Server server, PanelObject obj = null, Creature target = null);
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
            Spells = new Dictionary<string, SpellCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Skills = new Dictionary<string, SkillCreationDelegate>(StringComparer.CurrentCultureIgnoreCase);
            Effects = new Dictionary<string, OnUseDelegate>(StringComparer.CurrentCultureIgnoreCase);


            #region Items
            Effects.Add("NormalObj", new OnUseDelegate(NormalObj));

            Items.Add("Admin Trinket", new ItemCreationDelegate(AdminTrinket));
            Effects.Add("Admin Trinket", new OnUseDelegate(AdminTrinket));

            Items.Add("Test Item", new ItemCreationDelegate(TestItem));
            Effects.Add("Test Item", new OnUseDelegate(NormalObj));

            Items.Add("Test Male Equipment", new ItemCreationDelegate(TestMaleEquipment));
            Effects.Add("Test Male Equipment", new OnUseDelegate(Equip));

            Items.Add("Test Female Equipment", new ItemCreationDelegate(TestFemaleEquipment));
            Effects.Add("Test Female Equipment", new OnUseDelegate(Equip));
            #endregion

            #region Skills
            Skills.Add("Test Skill 1", new SkillCreationDelegate(TestSkill1));
            Effects.Add("Test Skill 1", new OnUseDelegate(TestSkill1));
            #endregion

            #region Spells
            Spells.Add("Mend", new SpellCreationDelegate(Mend));
            Effects.Add("Mend", new OnUseDelegate(Mend));

            Spells.Add("Heal", new SpellCreationDelegate(Heal));
            Effects.Add("Heal", new OnUseDelegate(Heal));

            Spells.Add("Srad Tut", new SpellCreationDelegate(SradTut));
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

        #region Defaults
        private void NormalObj(Client client, Server server, PanelObject obj = null, Creature target = null) { }
        private void Equip(Client client, Server server, PanelObject obj = null, Creature target = null)
        {
            if (obj == null)
                return;

            Item item = obj as Item;

            if (client.User.Exchange?.IsActive == true)
            {
                client.User.Exchange.AddItem(client.User, item.Slot);
                return;
            }

            if (!item.Gender.HasFlag(client.User.Gender))
            {
                client.SendServerMessage(ServerMessageType.ActiveMessage, "This item does not fit you.");
                return;
            }

            Item outItem;
            byte oldSlot = item.Slot;
            if(client.User.Equipment.TryEquip(item, out outItem))
            {
                client.User.Inventory.TryRemove(oldSlot);

                if(outItem != null)
                    client.Enqueue(ServerPackets.RemoveEquipment(outItem.EquipmentSlot));

                client.Enqueue(ServerPackets.RemoveItem(oldSlot));
                client.Enqueue(ServerPackets.AddEquipment(item));

                if (outItem != null && client.User.Inventory.AddToNextSlot(outItem))
                    client.Enqueue(ServerPackets.AddItem(outItem));

                foreach (User user in Game.World.ObjectsVisibleFrom(client.User).OfType<User>())
                    user.Client.Enqueue(ServerPackets.DisplayUser(client.User));

                client.Enqueue(ServerPackets.DisplayUser(client.User));
            }
        }
        #endregion

        #region Items
        private Item AdminTrinket(int count) => new Item(new ItemSprite(13709, 0), 0, "Admin Trinket", TimeSpan.Zero, 1, 1, Animation.None, BodyAnimation.None, true);
        private void AdminTrinket(Client client, Server server, PanelObject obj = null, Creature target = null)
        {
            if (obj == null)
                return;

            Item item = obj as Item;
            client.SendDialog(item, Game.Dialogs[item.NextDialogId]);
        }

        private Item TestItem(int count) => new Item(new ItemSprite(1108, 0), 0, "Test Item", true, count, 1, false);
        private Item TestMaleEquipment(int count) => new Item(new ItemSprite(1108, 208), 0, "Test Male Equipment", EquipmentSlot.Armor, 10000, 10000, 5, Gender.Male, false);
        private Item TestFemaleEquipment(int count) => new Item(new ItemSprite(1109, 208), 0, "Test Female Equipment", EquipmentSlot.Armor, 10000, 10000, 5, Gender.Female, false);
        #endregion

        #region Skills
        private Skill TestSkill1() => new Skill(0, 78, "Test Skill 1", SkillType.Front, TimeSpan.Zero, true, Animation.None, BodyAnimation.Assail);
        private void TestSkill1(Client client, Server server, PanelObject obj = null, Creature target = null)
        {
            if (obj == null)
                return;

            Skill skill = obj as Skill;

            Game.World.TryGetObject(client.User.Point.Offsetter(client.User.Direction), out target, client.User.Map);

            int amount = -50000;
            amount -= (client.User.Attributes.CurrentStr * 250);

            Game.Extensions.ApplySkill(client, amount, skill);
        }
        #endregion

        #region Spells
        private Spell Mend() => new Spell(0, 118, "Mend", SpellType.Targeted, string.Empty, 1, TimeSpan.Zero, new Animation(4, 0, 100), BodyAnimation.HandsUp);
        private void Mend(Client client, Server server, PanelObject obj = null, Creature target = null)
        {
            if (obj == null || target == null)
                return;

            Spell spell = obj as Spell;

            int amount = 10;
            amount += client.User.Attributes.CurrentWis * 5;

            Game.Extensions.ApplySpell(client, amount, spell, target);
        }

        private Spell Heal() => new Spell(0, 21, "Heal", SpellType.Targeted, string.Empty, 1, new TimeSpan(0, 0, 2), new Animation(157, 0, 100), BodyAnimation.HandsUp);
        private void Heal(Client client, Server server, PanelObject obj = null, Creature target = null)
        {
            if (obj == null || target == null)
                return;

            Spell spell = obj as Spell;

            Animation animation = new Animation(spell.EffectAnimation, target.Id, client.User.Id);
            int amount = 100000;
            amount += client.User.Attributes.CurrentWis * 500;

            Game.Extensions.ApplySpell(client, amount, spell, target);
        }
        private Spell SradTut() => new Spell(0, 21, "Srad Tut", SpellType.Targeted, string.Empty, 1, new TimeSpan(0, 0, 2), new Animation(158, 0, 100), BodyAnimation.HandsUp);
        private void SradTut(Client client, Server server, PanelObject obj = null, Creature target = null)
        {
            if (obj == null || target == null)
                return;

            Spell spell = obj as Spell;

            Animation animation = new Animation(spell.EffectAnimation, target.Id, client.User.Id);
            int amount = -100000;
            amount -= client.User.Attributes.CurrentWis * 500;

            Game.Extensions.ApplySpell(client, amount, spell, target);
        }
        #endregion
    }
}
