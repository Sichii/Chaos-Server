using System;
using System.Linq;

namespace Chaos
{
    internal sealed class Attributes
    {
        private Objects.User User { get; }
        //baseValues
        internal byte BaseStr { get; set; }
        internal byte BaseInt { get; set; }
        internal byte BaseWis { get; set; }
        internal byte BaseCon { get; set; }
        internal byte BaseDex { get; set; }
        internal uint BaseHP { get; set; }
        internal uint BaseMP { get; set; }

        //Primary
        internal byte Level { get; set; }
        internal byte Ability { get; set; }
        internal uint MaximumHP { get; set; }
        internal uint MaximumMP { get; set; }
        internal byte CurrentStr { get; set; }
        internal byte CurrentInt { get; set; }
        internal byte CurrentWis { get; set; }
        internal byte CurrentCon { get; set; }
        internal byte CurrentDex { get; set; }
        internal bool HasUnspentPoints { get; set; }
        internal byte UnspentPoints { get; set; }
        internal short MaximumWeight { get; set; }
        internal short CurrentWeight { get; set; }

        //Vitality
        internal uint CurrentHP { get; set; }
        internal uint CurrentMP { get; set; }

        //Experience
        internal uint Experience { get; set; }
        internal uint ToNextLevel { get; set; }
        internal uint AbilityExp { get; set; }
        internal uint ToNextAbility { get; set; }
        internal uint GamePoints { get; set; }
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

        internal Attributes(Objects.User user)
        {
            User = user;
        }
    }
}
