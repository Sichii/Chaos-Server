using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct Attributes
    {
        //baseValues
        internal byte BaseStr;
        internal byte BaseInt;
        internal byte BaseWis;
        internal byte BaseCon;
        internal byte BaseDex;
        internal uint BaseHP;
        internal uint BaseMP;

        //Primary
        internal byte Level;
        internal byte Ability;
        internal uint MaximumHP;
        internal uint MaximumMP;
        internal byte CurrentStr;
        internal byte CurrentInt;
        internal byte CurrentWis;
        internal byte CurrentCon;
        internal byte CurrentDex;
        internal bool HasUnspentPoints;
        internal byte UnspentPoints;
        internal short MaximumWeight;
        internal short CurrentWeight;

        //Vitality
        internal uint CurrentHP;
        internal uint CurrentMP;

        //Experience
        internal uint Experience;
        internal uint ToNextLevel;
        internal uint AbilityExp;
        internal uint ToNextAbility;
        internal uint GamePoints;
        internal uint Gold;

        //Secondary
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
