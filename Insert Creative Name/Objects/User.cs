using System;

namespace Insert_Creative_Name.Objects
{
    [Serializable]
    internal sealed class User : Creature
    {
        internal Panel<Skill> SkillBook { get; set; }
        internal Panel<Spell> SpellBook { get; set; }
        internal Panel<Item> Inventory { get; set; }
        internal DisplayData DisplayData { get; set; }
        internal Attributes Attributes { get; set; }
        internal Portrait Portrait { get; set; }
        internal Client Client { get; set; }

        internal User(uint id, string name, Point point, Map map, Direction direction)
          : base(id, name, 0, 4, point, map, direction)
        {
            DisplayData = new DisplayData();
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(60);
            Attributes = new Attributes();
        }

        internal User(uint id, string name, Point point, Map map, DisplayData displayData, Direction direction)
            :base(id, name, 0, 4, point, map, direction)
        {
            DisplayData = displayData;
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(60);
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
