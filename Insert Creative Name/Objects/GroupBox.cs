using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Objects
{
    internal sealed class GroupBox
    {
        internal User GroupLeader { get; set; }
        internal string Name { get; set; }
        internal byte MaxLevel { get; set; }
        internal byte[] MaxAmounts { get; set; }

        internal GroupBox(User leader, string groupName, byte maxLevel, byte[] maxAmounts)
        {
            GroupLeader = leader;
            Name = groupName;
            MaxLevel = maxLevel;
            MaxAmounts = maxAmounts;
        }
    }
}
