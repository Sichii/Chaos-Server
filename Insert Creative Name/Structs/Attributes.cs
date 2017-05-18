using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct Attributes
    {
        internal byte Level;
        internal byte Ability;
        internal uint MaximumHp;
        internal uint MaximumMp;
        internal uint BaseHp;
        internal uint BaseMp;
        internal byte Str;
        internal byte Int;
        internal byte Wis;
        internal byte Con;
        internal byte Dex;
        internal bool HasUnspentPoints;
        internal byte UnspentPoints;
        internal short MaximumWeight;
        internal short CurrentWeight;
        internal uint HP;
        internal uint MP;
        internal uint Experience;
        internal uint ToNextLevel;
        internal uint AbilityExp;
        internal uint ToNextAbility;
        internal uint GamePoints;
        internal uint Gold;
        internal byte Blind;
        internal MailFlags MailFlags;
        internal Element OffenseElement;
        internal Element DefenseElement;
        internal byte MagicResistance;
        internal sbyte ArmorClass;
        internal byte Dmg;
        internal byte Hit;
    }
}
