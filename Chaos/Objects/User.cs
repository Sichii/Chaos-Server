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
        [JsonProperty]
        internal Board MailBox { get; set; }
        [JsonProperty]
        internal Panel<Skill> SkillBook { get; set; }
        [JsonProperty]
        internal Panel<Spell> SpellBook { get; set; }
        [JsonProperty]
        internal Panel<Item> Inventory { get; set; }
        [JsonProperty]
        internal Panel<Item> Equipment { get; set; }
        [JsonProperty]
        internal IgnoreList IgnoreList { get; set; }
        [JsonProperty]
        internal UserOptions UserOptions { get; set; }
        [JsonProperty]
        internal DisplayData DisplayData { get; set; }
        [JsonProperty]
        internal Attributes Attributes { get; set; }
        [JsonProperty]
        internal Legend Legend { get; set; }
        [JsonProperty]
        internal Guild Guild { get; set; }
        [JsonProperty]
        internal SocialStatus SocialStatus { get; set; }
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
        internal Client Client { get; set; }
        internal Personal Personal { get; set; }
        internal bool IsGrouped => Group != null;
        internal Exchange Exchange { get; set; }
        internal bool ShouldDisplay => DateTime.UtcNow.Subtract(LastClicked).TotalMilliseconds < 500;
        internal override byte HealthPercent => Utility.Clamp<byte>((CurrentHP * 100) / MaximumHP, 0, (int)MaximumHP);
        internal override uint MaximumHP { get { return Attributes.MaximumHP; } }
        internal override uint CurrentHP { get { return Attributes.CurrentHP; } set { Attributes.CurrentHP = Attributes.CurrentHP == 0 ? 0 : value; } }
        internal byte ManaPercent => Utility.Clamp<byte>((CurrentMP * 100) / MaximumMP, 0, (int)MaximumMP);
        internal uint MaximumMP { get { return Attributes.MaximumMP; } }
        internal uint CurrentMP { get { return Attributes.CurrentMP; } set { Attributes.CurrentMP = value; } }

        internal DateTime LastClicked { get; set; }

        //user flags, use user.hasflag, addflag, removeflag
        [JsonProperty]
        private UserState State { get; set; }
        [JsonProperty]
        private Status Status { get; set; }
        [JsonProperty]
        private Quest Quest { get; set; }

        internal Location BlinkSpot { get; set; }

        /// <summary>
        /// Base constructor for an object representing a new in-game user, or player.
        /// </summary>
        internal User(string name, Point point, Map map, Direction direction, Gender gender)
            : this(name, point, map, direction, new Board(), new Panel<Skill>(90), new Panel<Spell>(90), new Panel<Item>(61), new Panel<Item>(20), new IgnoreList(),
                  new UserOptions(), null, new Attributes(), new EffectsBar(null), new Legend(), null, SocialStatus.Awake, Nation.None, BaseClass.Peasant, AdvClass.None,
                  false, null, new List<string>(), gender, UserState.None, Status.None, Quest.None, false)
        {
        }

        /// <summary>
        /// Json & Master constructor for an object representing an existing in-game user, or player.
        /// </summary>
        [JsonConstructor]
        private User(string name, Point point, Map map, Direction direction, Board mailBox, Panel<Skill> skillBook, Panel<Spell> spellBook, Panel<Item> inventory, 
            Panel<Item> equipment, IgnoreList ignoreList, UserOptions userOptions, DisplayData displayData, Attributes attributes, EffectsBar effectsBar, Legend legend, 
            Guild guild, SocialStatus socialStatus, Nation nation, BaseClass baseClass, AdvClass advClass, bool isMaster, string spouse, List<string> titles, 
            Gender gender, UserState state, Status status, Quest quest, bool isAdmin)
            : base(name, 0, CreatureType.User, point, map, direction, effectsBar)
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
            Guild = guild == null ? null : Game.World.Guilds[guild.Name];
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
            LastClicked = DateTime.MinValue;

            State = state;
            Status = status;
            Quest = quest;


            //re serialize all panels based on minimal information
            foreach(Item i in Inventory.ToList())
            {
                if (i == null) continue;
                Item newItem = Game.CreationEngine.CreateItem(i.Name);
                newItem.Count = i.Count;
                newItem.LastUse = i.LastUse;
                newItem.CurrentDurability = i.CurrentDurability;
                newItem.Slot = i.Slot;

                Inventory.TryRemove(i.Slot);
                inventory.TryAdd(newItem);
            }

            foreach (Item i in Equipment.ToList())
            {
                if (i == null) continue;
                Item newItem = Game.CreationEngine.CreateItem(i.Name);
                newItem.Count = i.Count;
                newItem.LastUse = i.LastUse;
                newItem.CurrentDurability = i.CurrentDurability;
                newItem.Slot = i.Slot;

                Inventory.TryRemove(i.Slot);
                inventory.TryAdd(newItem);
            }

            foreach (Spell s in SpellBook.ToList())
            {
                if (s == null) continue;
                Spell newSpell = Game.CreationEngine.CreateSpell(s.Name);
                newSpell.LastUse = s.LastUse;
                newSpell.Slot = s.Slot;

                SpellBook.TryRemove(s.Slot);
                SpellBook.TryAdd(newSpell);
            }

            foreach (Skill s in SkillBook.ToList())
            {
                if (s == null) continue;
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
            Map = Game.World.Maps[Map.Id];
        }

        /// <summary>
        /// Retreives a list of diagonal points in relevance to the user, with an optional distance and direction. Invalid direction returns all directions of diagonal points.
        /// </summary>
        internal List<Point> DiagonalPoints(int degree = 1, Direction direction = Direction.Invalid)
        {
            List<Point> diagonals = new List<Point>();

            for (int i = 1; i <= degree; i++)
            {
                switch (direction)
                {
                    case Direction.Invalid:
                        diagonals.Add(new Point((ushort)(Point.X - i), (ushort)(Point.Y - i)));
                        diagonals.Add(new Point((ushort)(Point.X + i), (ushort)(Point.Y - i)));
                        diagonals.Add(new Point((ushort)(Point.X + i), (ushort)(Point.Y + i)));
                        diagonals.Add(new Point((ushort)(Point.X - i), (ushort)(Point.Y + i)));
                        break;
                    case Direction.North:
                        diagonals.Add(new Point((ushort)(Point.X - i), (ushort)(Point.Y - i)));
                        diagonals.Add(new Point((ushort)(Point.X + i), (ushort)(Point.Y - i)));
                        break;
                    case Direction.East:
                        diagonals.Add(new Point((ushort)(Point.X + i), (ushort)(Point.Y - i)));
                        diagonals.Add(new Point((ushort)(Point.X + i), (ushort)(Point.Y + i)));
                        break;
                    case Direction.South:
                        diagonals.Add(new Point((ushort)(Point.X + i), (ushort)(Point.Y + i)));
                        diagonals.Add(new Point((ushort)(Point.X - i), (ushort)(Point.Y + i)));
                        break;
                    case Direction.West:
                        diagonals.Add(new Point((ushort)(Point.X - i), (ushort)(Point.Y - i)));
                        diagonals.Add(new Point((ushort)(Point.X - i), (ushort)(Point.Y + i)));
                        break;
                }
            }

            return diagonals;
        }

        /// <summary>
        /// Retreives a list of points in a line from the user, with an option for distance and direction. Invalid directions return an empty list.
        /// </summary>
        internal List<Point> LinePoints(int degree = 1, Direction direction = Direction.Invalid)
        {
            Point tempPoint = Point;
            List<Point> linePoints = new List<Point>();

            if (direction == Direction.Invalid)
                return linePoints;

            for (int i = 0; i < degree; i++)
            {
                tempPoint.Offset(direction);
                linePoints.Add(tempPoint);
            }

            return linePoints;
        }

        /// <summary>
        /// Saves the user to the database.
        /// </summary>
        internal void Save() => Client.Server.DataBase.TrySaveUser(this);

        /// <summary>
        /// Checks if the user has the given flag. Accepts Quest, UserState, and Status flags.
        /// </summary>
        internal bool HasFlag<T>(T flag) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum/flag.");

            Type t = typeof(T);
            Type quest;
            Type userState;
            Type status;

            if ((quest = typeof(Quest)) == t)
                return Quest.HasFlag((Quest)flag.ToType(quest, null));
            else if ((userState = typeof(UserState)) == t)
                return State.HasFlag((UserState)flag.ToType(userState, null));
            else if ((status = typeof(Status)) == t)
                return Status.HasFlag((Status)flag.ToType(status, null));
            else
                throw new ArgumentException("Invalid argument.");
        }

        /// <summary>
        /// Adds a flag to the user. Accepts Quest, UserState, and Status flags.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flag"></param>
        internal void AddFlag<T>(T flag) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum/flag.");

            Type t = typeof(T);
            Type quest;
            Type userState;
            Type status;

            if ((quest = typeof(Quest)) == t)
                Quest |= (Quest)flag.ToType(quest, null);
            else if ((userState = typeof(UserState)) == t)
                State |= (UserState)flag.ToType(userState, null);
            else if ((status = typeof(Status)) == t)
                Status |= (Status)flag.ToType(status, null);
            else
                throw new ArgumentException("Invalid argument.");
        }

        /// <summary>
        /// Removes a flag from the user. Accepts Quest, UserState, and Status flags.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flag"></param>
        internal void RemoveFlag<T>(T flag) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum/flag.");

            Type t = typeof(T);
            Type quest;
            Type userState;
            Type status;

            if ((quest = typeof(Quest)) == t)
                Quest &= ~(Quest)flag.ToType(quest, null);
            else if ((userState = typeof(UserState)) == t)
                State &= ~(UserState)flag.ToType(userState, null);
            else if ((status = typeof(Status)) == t)
                Status &= ~(Status)flag.ToType(status, null);
            else
                throw new ArgumentException("Invalid argument.");
        }
    }
}
