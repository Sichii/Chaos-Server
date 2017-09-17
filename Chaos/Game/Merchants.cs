using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal sealed class Merchants : IEnumerable<Merchant>
    {
        public IEnumerator<Merchant> GetEnumerator() => MerchantList.Values.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        //access merchant by name
        internal Merchant this[string name] => MerchantList[name];
        private Dictionary<string, Merchant> MerchantList { get; }

        internal Merchants()
        {
            MerchantList = new Dictionary<string, Merchant>()
            {
                {
                    "Deliope", new Merchant("Deliope", 61, CreatureType.Merchant, new Point(15, 10), Game.World.Maps[5031], Direction.South,
                    new List<PursuitIds>() { PursuitIds.Revive })
                }
            };
        }
    }
}
