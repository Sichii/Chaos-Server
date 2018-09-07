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
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class Attributes
    {
        internal User User { get; set; }

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
        internal int StrMod => User.EffectsBar.StrModSum;
        internal int IntMod => User.EffectsBar.IntModSum;
        internal int WisMod => User.EffectsBar.WisModSum;
        internal int ConMod => User.EffectsBar.ConModSum;
        internal int DexMod => User.EffectsBar.DexModSum;
        internal int MaxHPMod => User.EffectsBar.MaxHPModSum;
        internal int MaxMPMod => User.EffectsBar.MaxMPModSum;

        //Primary
        [JsonProperty]
        internal byte Level { get; set; }
        [JsonProperty]
        internal byte Ability { get; set; }

        internal uint MaximumHP => Utility.Clamp<uint>(BaseHP + MaxHPMod, 0, int.MaxValue);
        internal uint MaximumMP => Utility.Clamp<uint>(BaseMP + MaxMPMod, 0, int.MaxValue);
        internal byte CurrentStr => Utility.Clamp<byte>(BaseStr + StrMod, 0, byte.MaxValue);
        internal byte CurrentInt => Utility.Clamp<byte>(BaseInt + IntMod, 0, byte.MaxValue);
        internal byte CurrentWis => Utility.Clamp<byte>(BaseWis + WisMod, 0, byte.MaxValue);
        internal byte CurrentCon => Utility.Clamp<byte>(BaseCon + ConMod, 0, byte.MaxValue);
        internal byte CurrentDex => Utility.Clamp<byte>(BaseDex + DexMod, 0, byte.MaxValue);
        internal bool HasUnspentPoints => UnspentPoints != 0;
        [JsonProperty]
        internal byte UnspentPoints { get; set; }
        internal short MaximumWeight => (short)(40 + (BaseStr / 2));
        internal short CurrentWeight => (short)User.Inventory.Sum(item => item?.Weight ?? 0);

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
        internal byte Blind => 0;
        internal MailFlag MailFlags => MailFlag.None;
        internal Element OffenseElement => Element.None;
        internal Element DefenseElement => Element.None;
        internal byte MagicResistance => 0;
        internal sbyte ArmorClass => 50;
        internal byte Dmg => 0;
        internal byte Hit => 0;

        /// <summary>
        /// Default constructor for an object containing a new user's stats and flags.
        /// </summary>
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
            UnspentPoints = 0;
            CurrentHP = 100;
            CurrentMP = 100;
            Experience = 0;
            ToNextLevel = 150;
            AbilityExp = 0;
            ToNextAbility = 0;
            GamePoints = 0;
            Gold = 0;
        }

        /// <summary>
        /// Json & Master constructor for an object containing an existing character's stats and flags.
        /// </summary>
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
