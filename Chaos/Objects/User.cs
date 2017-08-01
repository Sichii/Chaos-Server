using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class User : Creature
    {
        [JsonProperty]
        internal Panel<Skill> SkillBook { get; set; }
        [JsonProperty]
        internal Panel<Spell> SpellBook { get; set; }
        [JsonProperty]
        internal Panel<Item> Inventory { get; set; }
        [JsonProperty]
        internal Panel<Item> Equipment { get; set; }
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
        internal bool Grouped => Group != null;

        internal User(string name, Point point, Map map, Direction direction)
            :base(name, 0, 4, point, map, direction)
        {
            SkillBook = new Panel<Skill>(90);
            SpellBook = new Panel<Spell>(90);
            Inventory = new Panel<Item>(61);
            Equipment = new Panel<Item>(20);
            UserOptions = new UserOptions();
            Attributes = new Attributes();
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
        }

        [JsonConstructor]
        internal User(string name, Point point, Map map, Direction direction, Panel<Skill> skillBook, Panel<Spell> spellBook, Panel<Item> inventory, Panel<Item> equipment, UserOptions userOptions, DisplayData displayData, Attributes attributes,
               Legend legend, Personal personal, Guild guild, SocialStatus socialStatus, Nation nation, BaseClass baseClass, AdvClass advClass, bool isMaster, string spouse, List<string> titles)
            :base(name, 0, 4, point, map)
        {
            SkillBook = skillBook;
            SpellBook = spellBook;
            Inventory = inventory;
            Equipment = equipment;
            UserOptions = userOptions;
            DisplayData = displayData;
            Attributes = attributes;
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
            Client = null;
            Group = null;
            DisplayData.User = this;
        }

        internal void Sync(Client client)
        {
            Client = client;
            Client.User = this;
            Map = client.Server.World.Maps[Map.Id];
        }

        internal void Save() => Client.Server.DataBase.TrySaveUser(this);
    }
}
