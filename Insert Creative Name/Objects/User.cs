using System;
using System.Collections.Generic;

namespace Insert_Creative_Name.Objects
{
    [Serializable]
    internal sealed class User : Creature
    {
        internal Panel<Skill> SkillBook { get; set; }
        internal Panel<Spell> SpellBook { get; set; }
        internal Panel<Item> Inventory { get; set; }
        internal Panel<Item> Equipment { get; set; }
        internal UserOptions Options { get; set; }
        internal DisplayData DisplayData { get; set; }
        internal Attributes Attributes { get; set; }
        internal Guild Guild { get; set; }
        internal Legend Legend { get; set; }
        internal Personal Personal { get; set; }
        internal Group Group { get; set; }
        internal Client Client { get; set; }
        internal SocialStatus SocialStatus { get; set; }
        internal Nation Nation { get; set; }
        internal BaseClass BaseClass { get; set; }
        internal AdvClass AdvClass { get; set; }
        internal MailFlag MailFlag { get; set; }
        internal bool Grouped { get; set; }
        internal bool IsMaster { get; set; }
        internal string Spouse { get; set; }
        internal List<string> Titles { get; set; }

        internal User(uint id, string name, Point point, Map map, Direction direction)
          : base(id, name, 0, 4, point, map, direction)
        {
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(61);
            Equipment = new Panel<Item>(20);
            Options = new UserOptions();
            DisplayData = new DisplayData();
            Attributes = new Attributes();
            Legend = new Legend();
            Titles = new List<string>();
            Group = null;
            Spouse = null;
        }

        internal User(uint id, string name, Point point, Map map, DisplayData displayData, Direction direction)
            :base(id, name, 0, 4, point, map, direction)
        {
            DisplayData = displayData;
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(61);
            Equipment = new Panel<Item>(20);
            Attributes = new Attributes();
        }

        internal void Resync(Client client)
        {
            Client = client;
            Map = client.Server.World.Maps[Map.Id];
        }

        private void Serialize()
        {
            Client = null;
            Data.Serialize($@"{Paths.Chars}\{Name}", this);
        }
    }
}
