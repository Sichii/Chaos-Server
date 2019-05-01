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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    /// <summary>
    /// Represents an in-game user, or player.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class User : Creature
    {
        //user flags, use user.hasflag, addflag, removeflag
        [JsonProperty]
        private UserState State { get; set; }
        [JsonProperty]
        private Quest Quest { get; set; }
        [JsonProperty]
        internal Board MailBox { get; }
        [JsonProperty]
        internal Panel<Skill> SkillBook { get; }
        [JsonProperty]
        internal Panel<Spell> SpellBook { get; }
        [JsonProperty]
        internal Panel<Item> Inventory { get; }
        [JsonProperty]
        internal Panel<Item> Equipment { get; }
        [JsonProperty]
        internal IgnoreList IgnoreList { get; }
        [JsonProperty]
        internal UserOptions UserOptions { get; }
        [JsonProperty]
        internal Attributes Attributes { get; }
        [JsonProperty]
        internal Legend Legend { get; }
        internal Client Client { get; private set; }
        [JsonProperty]
        internal DisplayData DisplayData { get; set; }
        [JsonProperty]
        internal SocialStatus SocialStatus { get; set; }
        [JsonProperty]
        internal string GuildName { get; set; }
        [JsonProperty]
        internal Nation Nation { get; set; }
        [JsonProperty]
        internal BaseClass BaseClass { get; set; }
        [JsonProperty]
        internal AdvClass AdvClass { get; set; }
        [JsonProperty]
        internal bool IsMaster { get; set; }
        [JsonProperty]
        internal string Spouse { get; set; }
        [JsonProperty]
        internal List<string> Titles { get; set; }
        [JsonProperty]
        internal bool IsAdmin { get; set; }
        [JsonProperty]
        internal Gender Gender { get; set; }

        internal Group Group { get; set; }
        internal Personal Personal { get; set; }
        internal int ExchangeID { get; set; }

        internal Exchange Exchange => Game.World.Exchanges.TryGetValue(ExchangeID, out Exchange outExchange) ? outExchange : null;
        internal Guild Guild => Game.World.Guilds.TryGetValue(GuildName ?? "", out Guild outGuild) ? outGuild : null;
        internal bool IsGrouped => Group != null;
        internal override byte HealthPercent => Utilities.Clamp<byte>(CurrentHP * 100 / MaximumHP, 0, (int)MaximumHP);
        internal override uint MaximumHP => Attributes.MaximumHP;
        internal override uint CurrentHP { get => Attributes.CurrentHP; set => Attributes.CurrentHP = (Attributes.CurrentHP == 0) ? 0 : value; }
        internal byte ManaPercent => Utilities.Clamp<byte>(CurrentMP * 100 / MaximumMP, 0, (int)MaximumMP);
        internal uint MaximumMP => Attributes.MaximumMP;
        internal uint CurrentMP { get => Attributes.CurrentMP; set => Attributes.CurrentMP = value; }

        //stuff for spells/skill, try to avoid doing this
        internal Location BlinkSpot { get; set; }

        /// <summary>
        /// Base constructor for an object representing a new in-game user, or player.
        /// </summary>
        internal User(string name, Location location, Direction direction, Gender gender)
            : this(name, location, direction, new Board(), new Panel<Skill>(PanelType.SkillBook), new Panel<Spell>(PanelType.SpellBook), new Panel<Item>(PanelType.Inventory), new Panel<Item>(PanelType.Equipment), new IgnoreList(),
                  new UserOptions(), null, new Attributes(), new EffectsBar(null), new Legend(), null, SocialStatus.Awake, Nation.None, BaseClass.Peasant, AdvClass.None,
                  false, null, new List<string>(), gender, UserState.None, Status.None, Quest.None, false)
        {
        }

        /// <summary>
        /// Json & Master constructor for an object representing an existing in-game user, or player.
        /// </summary>
        [JsonConstructor]
        private User(string name, Location location, Direction direction, Board mailBox, Panel<Skill> skillBook, Panel<Spell> spellBook, Panel<Item> inventory, 
            Panel<Item> equipment, IgnoreList ignoreList, UserOptions userOptions, DisplayData displayData, Attributes attributes, EffectsBar effectsBar, Legend legend, 
            string guildName, SocialStatus socialStatus, Nation nation, BaseClass baseClass, AdvClass advClass, bool isMaster, string spouse, List<string> titles, 
            Gender gender, UserState state, Status status, Quest quest, bool isAdmin)
            : base(name, location, 0, CreatureType.User, direction, effectsBar)
        {
            MailBox = mailBox;
            SkillBook = skillBook;
            SpellBook = spellBook;
            Inventory = inventory;
            Equipment = equipment;
            IgnoreList = ignoreList;
            UserOptions = userOptions;
            DisplayData = displayData;
            Attributes = attributes;
            Attributes.User = this;
            Legend = legend;
            GuildName = guildName;
            SocialStatus = socialStatus;
            Nation = nation;
            BaseClass = baseClass;
            AdvClass = advClass;
            IsMaster = isMaster;
            Spouse = spouse;
            Titles = titles;
            Gender = gender;
            Client = null;
            Group = null;
            Personal = null;

            if (DisplayData != null)
                DisplayData.User = this;

            IsAdmin = isAdmin;

            State = state;
            Status = status;
            Quest = quest;


            //re serialize all panels based on minimal information
            foreach(Item i in Inventory.ToList())
            {
                Item newItem = Game.CreationEngine.CreateItem(i.Name);
                newItem.Color = i.Color;
                newItem.Count = i.Count;
                newItem.LastUse = i.LastUse;
                newItem.CurrentDurability = i.CurrentDurability;
                newItem.Slot = i.Slot;

                Inventory.TryRemove(i.Slot);
                Inventory.TryAdd(newItem);
            }

            foreach (Item i in Equipment.ToList())
            {
                Item newItem = Game.CreationEngine.CreateItem(i.Name);
                newItem.Color = i.Color;
                newItem.Count = i.Count;
                newItem.LastUse = i.LastUse;
                newItem.CurrentDurability = i.CurrentDurability;
                newItem.Slot = i.Slot;

                Equipment.TryRemove(i.Slot);
                Equipment.TryAdd(newItem);
            }

            foreach (Spell s in SpellBook.ToList())
            {
                Spell newSpell = Game.CreationEngine.CreateSpell(s.Name);
                newSpell.LastUse = s.LastUse;
                newSpell.Slot = s.Slot;

                SpellBook.TryRemove(s.Slot);
                SpellBook.TryAdd(newSpell);
            }

            foreach (Skill s in SkillBook.ToList())
            {
                Skill newSkill = Game.CreationEngine.CreateSkill(s.Name);
                newSkill.LastUse = s.LastUse;
                newSkill.Slot = s.Slot;

                SkillBook.TryRemove(s.Slot);
                SkillBook.TryAdd(newSkill);
            }
        }

        /// <summary>
        /// Re-synchronizes the user to a client.
        /// </summary>
        internal void Resync(Client client)
        {
            Client = client;
            Client.User = this;
        }

        /// <summary>
        /// Clears all skills and spells from the user.
        /// </summary>
        internal void ClearSkillsSpells()
        {
            foreach (Skill skill in SkillBook.Where(s => s != null).ToList())
                if (SkillBook.TryRemove(skill.Slot))
                    Client.Enqueue(ServerPackets.RemoveSkill(skill.Slot));

            foreach (Spell spell in SpellBook.Where(s => s != null).ToList())
                if (spell.Name != "Admin Create" && SpellBook.TryRemove(spell.Slot))
                    Client.Enqueue(ServerPackets.RemoveSpell(spell.Slot));
        }

        /// <summary>
        /// Saves the user to the database.
        /// </summary>
        internal void Save() => Client.Server.DataBase.TrySaveUser(this);

        /// <summary>
        /// Checks if the user has the given flag. Accepts Quest, UserState, and Status flags.
        /// </summary>
        internal bool HasFlag<T>(T flag) where T : Enum
        {
            if (flag is Quest)
                return Quest.HasFlag(flag);
            else if (flag is UserState)
                return State.HasFlag(flag);
            else if (flag is Status)
                return Status.HasFlag(flag);
            else
                throw new ArgumentException("Invalid argument.");
        }

        /// <summary>
        /// Adds a flag to the user. Accepts Quest, UserState, and Status flags.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flag"></param>
        internal void AddFlag<T>(T flag) where T : Enum
        {
            if (flag is Quest questFlag)
                Quest |= questFlag;
            else if (flag is UserState stateFlag)
                State |= stateFlag;
            else if (flag is Status statusFlag)
                Status |= statusFlag;
            else
                throw new ArgumentException("Invalid argument.");
        }

        /// <summary>
        /// Removes a flag from the user. Accepts Quest, UserState, and Status flags.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flag"></param>
        internal void RemoveFlag<T>(T flag) where T : Enum
        {
            if (flag is Quest questFlag)
                Quest &= ~questFlag;
            else if (flag is UserState stateFlag)
                State &= ~stateFlag;
            else if (flag is Status statusFlag)
                Status &= ~statusFlag;
            else
                throw new ArgumentException("Invalid argument.");
        }
    }
}
