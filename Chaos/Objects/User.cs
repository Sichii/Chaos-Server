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

namespace Chaos
{
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
        internal Personal Personal { get; set; }
        [JsonProperty]
        internal Guild Guild { get; set; }
        [JsonIgnore]
        internal Group Group { get; set; }
        [JsonIgnore]
        internal Client Client { get; set; }
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
        internal bool IsChanting { get; set; }
        internal bool IsGrouped => Group != null;
        internal Exchange Exchange { get; set; }
        internal DateTime LastClicked { get; set; }
        internal bool ShouldDisplay => DateTime.UtcNow.Subtract(LastClicked).TotalMilliseconds < 500;
        internal override byte HealthPercent => Utility.Clamp<byte>((int)((CurrentHP * 100) / MaximumHP), 0, (int)MaximumHP);
        internal override uint MaximumHP { get { return Attributes.MaximumHP; } }
        internal override uint CurrentHP { get { return Attributes.CurrentHP; } set { Attributes.CurrentHP = value; } }
        internal bool DeathDisplayed { get; set; }
        internal int BlinkCount { get; set; }
        internal Location BlinkSpot { get; set; }

        internal User(Gender gender, string name, Point point, Map map, Direction direction)
            : base(name, 0, CreatureType.User, point, map, direction)
        {
            Gender = gender;
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(61);
            Equipment = new Panel<Item>(20);
            IgnoreList = new IgnoreList();
            UserOptions = new UserOptions();
            Attributes = new Attributes();
            Attributes.User = this;
            Legend = new Legend();
            Group = null;
            Spouse = null;
            BaseClass = BaseClass.Peasant;
            AdvClass = AdvClass.None;
            Nation = Nation.None;
            SocialStatus = SocialStatus.Awake;
            IsMaster = false;
            Spouse = string.Empty;
            Titles = new List<string>();
            IsAdmin = false;
            IsChanting = false;
            LastClicked = DateTime.MinValue;
            MailBox = new Board();
            DeathDisplayed = false;
        }

        [JsonConstructor]
        internal User(string name, Point point, Map map, Direction direction, Board mailBox, Panel<Skill> skillBook, Panel<Spell> spellBook, Panel<Item> inventory, Panel<Item> equipment, IgnoreList ignoreList, UserOptions userOptions, DisplayData displayData, Attributes attributes,
               Legend legend, Personal personal, Guild guild, SocialStatus socialStatus, Nation nation, BaseClass baseClass, AdvClass advClass, bool isMaster, string spouse, List<string> titles, Gender gender, bool isAdmin)
            : base(name, 0, CreatureType.User, point, map)
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
            Personal = personal;
            Guild = guild;
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
            DisplayData.User = this;
            IsAdmin = isAdmin;
            IsChanting = false;
            LastClicked = DateTime.MinValue;
            DeathDisplayed = !IsAlive;
        }

        internal void Resync(Client client)
        {
            Client = client;
            Client.User = this;
            Map = Game.World.Maps[Map.Id];
        }

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

        internal List<Point> LinePoints(int degree = 1, Direction direction = Direction.Invalid)
        {
            Point tempPoint = Point;
            List<Point> linePoints = new List<Point>();

            for(int i = 0; i < degree; i++)
            {
                tempPoint.Offset(direction);
                linePoints.Add(tempPoint);
            }

            return linePoints;
        }

        internal void Save() => Client.Server.DataBase.TrySaveUser(this);
    }
}
