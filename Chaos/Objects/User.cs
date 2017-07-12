using System;
using System.Collections.Generic;

namespace Chaos.Objects
{
    internal sealed class User : Creature
    {
        internal Panel<Skill> SkillBook { get; set; }
        internal Panel<Spell> SpellBook { get; set; }
        internal Panel<Item> Inventory { get; set; }
        internal Panel<Item> Equipment { get; set; }
        internal UserOptions Options { get; set; }
        internal DisplayData DisplayData { get; set; }
        internal Attributes Attributes { get; set; }
        internal Legend Legend { get; set; }
        internal Personal Personal { get; set; }
        internal Guild Guild { get; set; }
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

        internal User(string name, Point point, Map map, DisplayData displayData, Direction direction)
            :base(name, 0, 4, point, map, direction)
        {
            DisplayData = displayData;
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(61);
            Equipment = new Panel<Item>(20);
            Attributes = new Attributes(this);
            Options = new UserOptions();
            Legend = new Legend();
            Titles = new List<string>();
            Group = null;
            Spouse = null;
            DisplayData = displayData;
        }
    }
}
