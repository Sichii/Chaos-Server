using Newtonsoft.Json;
using System;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
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

        //addedValues
        internal byte AddedStr { get; set; }
        internal byte AddedInt { get; set; }
        internal byte AddedWis { get; set; }
        internal byte AddedCon { get; set; }
        internal byte AddedDex { get; set; }
        internal byte AddedHP { get; set; }
        internal byte AddedMP { get; set; }

        //Primary
        [JsonProperty]
        internal byte Level { get; set; }
        [JsonProperty]
        internal byte Ability { get; set; }

        internal uint MaximumHP => BaseHP + AddedHP;
        internal uint MaximumMP => BaseMP + AddedMP;
        internal byte CurrentStr => (byte)(BaseStr + AddedStr);
        internal byte CurrentInt => (byte)(BaseInt + AddedInt);
        internal byte CurrentWis => (byte)(BaseWis + AddedWis);
        internal byte CurrentCon => (byte)(BaseCon + AddedCon);
        internal byte CurrentDex => (byte)(BaseDex + AddedDex);
        internal bool HasUnspentPoints => UnspentPoints != 0;

        [JsonProperty]
        internal byte UnspentPoints { get; set; }
        internal short MaximumWeight => (short)(40 + (BaseStr / 2));
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
        internal byte Blind { get; set; }
        internal MailFlag MailFlags { get; set; }
        internal Element OffenseElement { get; set; }
        internal Element DefenseElement { get; set; }
        internal byte MagicResistance { get; set; }
        internal sbyte ArmorClass { get; set; }
        internal byte Dmg { get; set; }
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
            AddedHP = 0;
            AddedMP = 0;
            AddedStr = 0;
            AddedInt = 0;
            AddedWis = 0;
            AddedCon = 0;
            AddedDex = 0;
            UnspentPoints = 0;
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
        internal Attributes(byte baseStr, byte baseInt, byte baseWis, byte baseCon, byte baseDex, uint baseHp, uint baseMp, byte level, byte ability, byte unspentPoints, uint currentHP, uint currentMP, uint experience, uint toNextLevel, uint abilityExp, uint toNextAbility, uint gamePoints, uint gold)
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
            UnspentPoints = unspentPoints;
            CurrentHP = currentHP;
            CurrentMP = currentMP;
            Experience = experience;
            ToNextLevel = toNextLevel;
            AbilityExp = abilityExp;
            ToNextAbility = toNextAbility;
            GamePoints = gamePoints;
            Gold = gold;
        }
    }
}
