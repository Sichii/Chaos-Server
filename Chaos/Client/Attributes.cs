using Newtonsoft.Json;
using System;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Attributes
    {
        //baseValues
        [JsonProperty]
        internal byte BaseStr { get; set; }
        [JsonProperty]
        internal byte BaseInt { get; set; }
        [JsonProperty]
        internal byte BaseWis { get; set; }
        [JsonProperty]
        internal byte BaseCon { get; set; }
        [JsonProperty]
        internal byte BaseDex { get; set; }
        [JsonProperty]
        internal uint BaseHP { get; set; }
        [JsonProperty]
        internal uint BaseMP { get; set; }

        //Primary
        [JsonProperty]
        internal byte Level { get; set; }
        [JsonProperty]
        internal byte Ability { get; set; }
        [JsonProperty]
        internal uint MaximumHP { get; set; }
        [JsonProperty]
        internal uint MaximumMP { get; set; }
        [JsonProperty]
        internal byte CurrentStr { get; set; }
        [JsonProperty]
        internal byte CurrentInt { get; set; }
        [JsonProperty]
        internal byte CurrentWis { get; set; }
        [JsonProperty]
        internal byte CurrentCon { get; set; }
        [JsonProperty]
        internal byte CurrentDex { get; set; }
        internal bool HasUnspentPoints => UnspentPoints != 0;
        [JsonProperty]
        internal byte UnspentPoints { get; set; }
        [JsonProperty]
        internal short MaximumWeight { get; set; }
        [JsonProperty]
        internal short CurrentWeight { get; set; }

        //Vitality
        [JsonProperty]
        internal uint CurrentHP { get; set; }
        [JsonProperty]
        internal uint CurrentMP { get; set; }

        //Experience
        [JsonProperty]
        internal uint Experience { get; set; }
        [JsonProperty]
        internal uint ToNextLevel { get; set; }
        [JsonProperty]
        internal uint AbilityExp { get; set; }
        [JsonProperty]
        internal uint ToNextAbility { get; set; }
        [JsonProperty]
        internal uint GamePoints { get; set; }
        [JsonProperty]
        internal uint Gold { get; set; }

        //Secondary
        [JsonProperty]
        internal byte Blind { get; set; }
        [JsonProperty]
        internal MailFlag MailFlags { get; set; }
        [JsonProperty]
        internal Element OffenseElement { get; set; }
        [JsonProperty]
        internal Element DefenseElement { get; set; }
        [JsonProperty]
        internal byte MagicResistance { get; set; }
        [JsonProperty]
        internal sbyte ArmorClass { get; set; }
        [JsonProperty]
        internal byte Dmg { get; set; }
        [JsonProperty]
        internal byte Hit { get; set; }

        internal Attributes()
        {
            BaseStr = 3;
            BaseInt = 3;
            BaseWis = 3;
            BaseCon = 3;
            BaseDex = 3;
            BaseHP = 100;
            BaseMP = 100;
            Level = 1;
            Ability = 0;
            MaximumHP = 100;
            MaximumMP = 100;
            CurrentStr = 3;
            CurrentInt = 3;
            CurrentWis = 3;
            CurrentCon = 3;
            CurrentDex = 3;
            UnspentPoints = 0;
            MaximumWeight = 50;
            CurrentWeight = 0;
            CurrentHP = 100;
            CurrentMP = 100;
            Experience = 0;
            ToNextLevel = 150;
            AbilityExp = 0;
            ToNextAbility = 0;
            GamePoints = 0;
            Gold = 0;
            Blind = 0;
            MailFlags = MailFlag.None;
            OffenseElement = Element.None;
            DefenseElement = Element.None;
            MagicResistance = 0;
            ArmorClass = 0;
            Dmg = 0;
            Hit = 0;
        }

        [JsonConstructor]
        internal Attributes(byte baseStr, byte baseInt, byte baseWis, byte baseCon, byte baseDex, uint baseHp, uint baseMp, byte level, byte ability)
        {
            BaseStr = baseStr;
            BaseInt = baseInt;
            BaseWis = baseWis;
            BaseCon = baseCon;
            BaseDex = baseDex;
            BaseHP = baseHp;
            BaseMP = baseMp;
            Level = level;
            Ability = ability;
        }
    }
}
