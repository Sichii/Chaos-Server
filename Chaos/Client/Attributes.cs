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
        internal User User;

        //baseValues
        [JsonProperty]
        internal byte BaseStr;
        [JsonProperty]
        internal byte BaseInt;
        [JsonProperty]
        internal byte BaseWis;
        [JsonProperty]
        internal byte BaseCon;
        [JsonProperty]
        internal byte BaseDex;
        [JsonProperty]
        internal uint BaseHP;
        [JsonProperty]
        internal uint BaseMP;

        //addedValues
        internal sbyte StrMod;
        internal sbyte IntMod;
        internal sbyte WisMod;
        internal sbyte ConMod;
        internal sbyte DexMod;
        internal int HPMod;
        internal int MPMod;

        //Primary
        [JsonProperty]
        internal byte Level;
        [JsonProperty]
        internal byte Ability;

        internal uint MaximumHP => Utility.Clamp<uint>((int)(BaseHP + HPMod), 0, int.MaxValue);
        internal uint MaximumMP => Utility.Clamp<uint>((int)(BaseMP + MPMod), 0, int.MaxValue);
        internal byte CurrentStr => Utility.Clamp<byte>(BaseStr + StrMod, 0, byte.MaxValue);
        internal byte CurrentInt => Utility.Clamp<byte>(BaseInt + IntMod, 0, byte.MaxValue);
        internal byte CurrentWis => Utility.Clamp<byte>(BaseWis + WisMod, 0, byte.MaxValue);
        internal byte CurrentCon => Utility.Clamp<byte>(BaseCon + ConMod, 0, byte.MaxValue);
        internal byte CurrentDex => Utility.Clamp<byte>(BaseDex + DexMod, 0, byte.MaxValue);
        internal bool HasUnspentPoints => UnspentPoints != 0;

        [JsonProperty]
        internal byte UnspentPoints;
        internal short MaximumWeight => (short)(40 + (BaseStr / 2));
        internal short CurrentWeight => (short)User.Inventory.Sum(item => item?.Weight ?? 0);

        //Vitality
        [JsonProperty]
        internal uint CurrentHP;
        [JsonProperty]
        internal uint CurrentMP;

        //Experience
        [JsonProperty]
        internal uint Experience;
        [JsonProperty]
        internal uint ToNextLevel;
        [JsonProperty]
        internal uint AbilityExp;
        [JsonProperty]
        internal uint ToNextAbility;
        [JsonProperty]
        internal uint GamePoints;
        [JsonProperty]
        internal uint Gold;

        //Secondary
        internal byte Blind;
        internal MailFlag MailFlags;
        internal Element OffenseElement;
        internal Element DefenseElement;
        internal byte MagicResistance;
        internal sbyte ArmorClass;
        internal byte Dmg;
        internal byte Hit;

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
            HPMod = 0;
            MPMod = 0;
            StrMod = 0;
            IntMod = 0;
            WisMod = 0;
            ConMod = 0;
            DexMod = 0;
            UnspentPoints = 0;
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
