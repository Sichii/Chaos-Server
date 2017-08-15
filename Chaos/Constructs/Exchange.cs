using System.Collections.Generic;
using System.Threading;

namespace Chaos
{
    internal sealed class Exchange
    {
        internal Server Server { get; }
        internal int ExchangeId { get; }
        internal User User1 { get; }
        internal User User2 { get; }
        internal List<Item> User1Items { get; set; }
        internal List<Item> User2Items { get; set; }
        internal uint User1Gold;
        internal uint User2Gold;

        internal Exchange(User sender, User receiver)
        {
            ExchangeId = Interlocked.Increment(ref Server.NextId);
            User1 = sender;
            User2 = receiver;

            User1Items = new List<Item>();
            User2Items = new List<Item>();
            User1Gold = 0;
            User2Gold = 0;

            Server = sender.Client.Server;
        }

        internal void Activate(Item item = null, uint amount = 0)
        {
            List<object> args1 = new List<object>() { ExchangeId, User2.Name };
            List<object> args2 = new List<object>() { ExchangeId, User1.Name };

            User1.Client.Enqueue(Server.Packets.Exchange(ExchangeType.BeginTrade, args1));
            User2.Client.Enqueue(Server.Packets.Exchange(ExchangeType.BeginTrade, args2));

            if(item != null)
            {
                if(!item.Stackable)
                {
                    args1 = new List<object>() { };
                    args2 = new List<object>() { };

                    User1.Client.Enqueue(Server.Packets.Exchange(ExchangeType.AddNonStackable));
                }
            }
        }

        internal void AddItem(User user, Item item)
        {
            if (User1 == user)
                User1Items.Add(item);
            else
                User2Items.Add(item);
        }

        internal void SetGold(User user, uint amount)
        {
            if (User1 == user)
                User1Gold = amount;
            else
                User2Gold = amount;
        }
    }
}
