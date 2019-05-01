// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General internal License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Chaos
{
    internal sealed class Exchange
    {
        private readonly object Sync = new object();
        private readonly User User1;
        private readonly User User2;
        private uint User1Gold;
        private uint User2Gold;
        private bool User1Accept;
        private bool User2Accept;
        private List<Item> User1Items;
        private List<Item> User2Items;

        internal User OtherUser(User user) => user == User1 ? User2 : User1;
        internal int ID { get; }
        internal bool IsActive { get; private set; }

        /// <summary>
        /// Base constructor for an object representing an in-game exchange, or trade.
        /// </summary>
        /// <param name="sender">The user who requested the trade.</param>
        /// <param name="receiver">The user to receive the trade.</param>
        internal Exchange(User sender, User receiver)
        {
            ID = Interlocked.Increment(ref Server.NextID);
            User1 = sender;
            User2 = receiver;
            User1Items = new List<Item>();
            User2Items = new List<Item>();
            User1Gold = 0;
            User2Gold = 0;
        }

        /// <summary>
        /// Synchronously activates the exchange window, setting relevant variables and sending the packet to create the window on each client.
        /// </summary>
        internal void Activate()
        {
            lock (Sync)
            {
                //set each player's exchange to this
                User1.ExchangeID = ID;
                User2.ExchangeID = ID;

                //active exchange window on each client
                User1.Client.Enqueue(ServerPackets.Exchange(ExchangeType.StartExchange, User2.ID, User2.Name));
                User2.Client.Enqueue(ServerPackets.Exchange(ExchangeType.StartExchange, User1.ID, User1.Name));

                IsActive = true;
            }
        }

        /// <summary>
        /// Synchronously adds an item from the user's inventory to the trade window. Sends a prompt for stackable items. Updates both user's screens with the info.
        /// </summary>
        /// <param name="user">The user who is adding the item.</param>
        /// <param name="slot">The slot that item is in.</param>
        internal void AddItem(User user, byte slot)
        {
            lock (Sync)
            {
                byte index;
                bool user1Src = user == User1;
                User otherUser = OtherUser(user);

                if (!IsActive || !user.Inventory.TryGetObject(slot, out Item item) || (user1Src ? User1Accept : User2Accept))
                    return;

                if (item.AccountBound)
                {
                    user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{item.Name} is account bound.");
                    return;
                }
                if (otherUser.Inventory.AvailableSlots == 0 && (!item.Stackable || !User1Items.Contains(item)))
                {
                    user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{otherUser.Name} cannot hold any more items.");
                    otherUser.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You cannot hold any more items.");
                    return;
                }

                if (!item.Stackable)
                {
                    //remove item from their inventory
                    user.Inventory.TryRemove(slot);
                    user.Client.Enqueue(ServerPackets.RemoveItem(slot));

                    //add item to exchange
                    if (user1Src)
                    {
                        User1Items.Add(item);
                        index = (byte)(User1Items.IndexOf(item) + 1);
                    }
                    else
                    {
                        User2Items.Add(item);
                        index = (byte)(User2Items.IndexOf(item) + 1);
                    }

                    //update exchange window

                    User1.Client.Enqueue(ServerPackets.Exchange(ExchangeType.AddItem, !user1Src, index, item.ItemSprite.OffsetSprite, item.Color, item.Name));
                    User2.Client.Enqueue(ServerPackets.Exchange(ExchangeType.AddItem, user1Src, index, item.ItemSprite.OffsetSprite, item.Color, item.Name));
                }
                else //if it's stackable, send a prompty asking for how many
                    user.Client.Enqueue(ServerPackets.Exchange(ExchangeType.RequestAmount, item.Slot));
            }
        }

        /// <summary>
        /// Synchronously adds a stackable item to the trade. Updates both user's screens with the info. This method is requested after the user replys to the prompt from AddItem for stackable items.
        /// </summary>
        /// <param name="user">The user who is adding the item.</param>
        /// <param name="slot">The slot that item is in.</param>
        /// <param name="count">The number of that item to add.</param>
        internal void AddStackableItem(User user, byte slot, byte count)
        {
            lock (Sync)
            {
                Item splitItem;
                int index;
                bool user1Src = user == User1;
                User otherUser = OtherUser(user);

                //if slot is null, or not stackable, or invalid count, then return
                if (!IsActive || !user.Inventory.TryGetObject(slot, out Item item) || !item.Stackable || count > item.Count || item.AccountBound || (user1Src ? User1Accept : User2Accept))
                    return;

                if(otherUser.Inventory.Contains(item) && (otherUser.Inventory[item.Name].Count + count) > CONSTANTS.ITEM_STACK_MAX)
                {
                    user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{otherUser.Name} cannot hold that many {item.Name}.");
                    return;
                }

                //remove the item if we're exchanging all that we have
                if (item.Count == count)
                {
                    user.Inventory.TryGetRemove(slot, out splitItem);
                    user.Client.Enqueue(ServerPackets.RemoveItem(slot));
                }
                else
                {
                    //otherwise split the item stack and update the user's inventory
                    splitItem = item.Split(count);
                    user.Client.Enqueue(ServerPackets.AddItem(item));
                }

                //depending on which user is activating this, do different things
                if (user1Src)
                {
                    //if there's a stackable item with this name already, grab it
                    Item oldItem = User1Items.FirstOrDefault(itm => itm.Name.Equals(splitItem.Name));

                    //if it was successfully grabbed
                    if (oldItem != null)
                    {
                        //combine what we took from the inventory with the old amount
                        splitItem.Count += oldItem.Count;
                        //set the old item as the new item with the new count
                        User1Items[User1Items.IndexOf(oldItem)] = splitItem;
                    }
                    else
                        //if no old item, just add the new item stack
                        User1Items.Add(splitItem);

                    //index of this item inthe trade is it's index in the item list + 1 (cuz no zero index)
                    index = User1Items.IndexOf(splitItem) + 1;
                }
                else
                {
                    //do the same thing for the other use, using it's lists
                    Item oldItem = User2Items.FirstOrDefault(itm => itm.Name.Equals(splitItem.Name));

                    if (oldItem != null)
                    {
                        splitItem.Count += oldItem.Count;
                        User2Items[User2Items.IndexOf(oldItem)] = splitItem;
                    }
                    else
                        User2Items.Add(splitItem);

                    index = User2Items.IndexOf(splitItem) + 1;
                }

                //update exchange window
                User1.Client.Enqueue(ServerPackets.Exchange(ExchangeType.AddItem, !user1Src, (byte)index, splitItem.ItemSprite.OffsetSprite, splitItem.Color, $@"{splitItem.Name}[{splitItem.Count}]"));
                User2.Client.Enqueue(ServerPackets.Exchange(ExchangeType.AddItem, user1Src, (byte)index, splitItem.ItemSprite.OffsetSprite, splitItem.Color, $@"{splitItem.Name}[{splitItem.Count}]"));
            }
        }

        /// <summary>
        /// Synchronously sets the gold to be traded. Does necessary checks and modifications.
        /// </summary>
        /// <param name="user">User whos gold should be set.</param>
        /// <param name="amount">The total amount of gold they want to trade.</param>
        internal void SetGold(User user, uint amount)
        {
            lock (Sync)
            {
                bool user1Src = user == User1;

                if (!IsActive || (user1Src ? User1Accept : User2Accept))
                    return;

                //if the user already had gold entered, give it back (because this is a set, not an addition)
                user.Attributes.Gold += user1Src ? User1Gold : User2Gold;

                //if the amount they want to set is greater than what they have, set it to the max value
                if (amount > user.Attributes.Gold)
                    amount = user.Attributes.Gold;

                //do things depending on which user is requesting
                if (user1Src)
                {
                    //subtract the gold we want to add from the user
                    User1.Attributes.Gold -= amount;
                    //set the gold in the exchange
                    User1Gold = amount;
                    //update the user's gold
                    User1.Client.SendAttributes(StatUpdateType.ExpGold);
                }
                else
                {
                    //do same thing for other user
                    User2.Attributes.Gold -= amount;
                    User2Gold = amount;
                    User2.Client.SendAttributes(StatUpdateType.ExpGold);
                }

                //update exchange window
                User1.Client.Enqueue(ServerPackets.Exchange(ExchangeType.SetGold, !user1Src, user1Src ? User1Gold : User2Gold));
                User2.Client.Enqueue(ServerPackets.Exchange(ExchangeType.SetGold, user1Src, user1Src ? User1Gold : User2Gold));
            }
        }

        /// <summary>
        /// Synchronously cancels the trade, returning any gold and items that were added to the trade to their owners.
        /// </summary>
        /// <param name="user"></param>
        internal void Cancel(User user)
        {
            lock(Sync)
            {
                if (!IsActive)
                    return;

                //give the items and gold back to their owners
                User1.Attributes.Gold += User1Gold;
                User1.Client.SendAttributes(StatUpdateType.ExpGold);
                foreach (Item item in User1Items)
                {
                    User1.Inventory.TryAdd(item);
                    User1.Client.Enqueue(ServerPackets.AddItem(item));
                }

                User2.Attributes.Gold += User2Gold;
                User2.Client.SendAttributes(StatUpdateType.ExpGold);
                foreach (Item item in User2Items)
                {
                    User2.Inventory.TryAdd(item);
                    User2.Client.Enqueue(ServerPackets.AddItem(item));
                }

                //send cancel packet to close the exchange
                bool user1Src = user == User1;
                User1.Client.Enqueue(ServerPackets.Exchange(ExchangeType.Cancel, !user1Src));
                User2.Client.Enqueue(ServerPackets.Exchange(ExchangeType.Cancel, user1Src));

                //destroy the exchange object from the server
                Destroy();
            }
        }

        /// <summary>
        /// Synchronously accepts the trade. If both users have accepted, the added items and gold are of each user are added to the other user's inventory.
        /// </summary>
        /// <param name="user">The user who accepted the trade.</param>
        internal void Accept(User user)
        {
            lock(Sync)
            {
                if (!IsActive)
                    return;

                bool user1Src = user == User1;

                //keep track of which user has hit accept
                if (user1Src)
                    User1Accept = true;
                else
                    User2Accept = true;

                //only send the opposite user a true accept packet (accept button on other side)
                if (user1Src)
                    User2.Client.Enqueue(ServerPackets.Exchange(ExchangeType.Accept, true));
                else
                    User1.Client.Enqueue(ServerPackets.Exchange(ExchangeType.Accept, true));

                //if both players accepted, give eachother the items and gold in eachother's lists
                if (User1Accept && User2Accept)
                {
                    User2.Attributes.Gold += User1Gold;
                    User2.Client.SendAttributes(StatUpdateType.ExpGold);
                    foreach (Item item in User1Items)
                    {
                        User2.Inventory.AddToNextSlot(item);
                        User2.Client.Enqueue(ServerPackets.AddItem(item));
                    }

                    User1.Attributes.Gold += User2Gold;
                    User1.Client.SendAttributes(StatUpdateType.ExpGold);
                    foreach (Item item in User2Items)
                    {
                        User1.Inventory.AddToNextSlot(item);
                        User1.Client.Enqueue(ServerPackets.AddItem(item));
                    }

                    //update exchange window (to close it)
                    User1.Client.Enqueue(ServerPackets.Exchange(ExchangeType.Accept, false));
                    User2.Client.Enqueue(ServerPackets.Exchange(ExchangeType.Accept, false));

                    //destroy the exchange object from the server
                    Destroy();
                }
            }
        }

        /// <summary>
        /// Destroys the exchange. Should only be called from cancel, to avoid losing items and gold to the abyss.
        /// </summary>
        private void Destroy()
        {
            //remove the exchange from existence
            User1.ExchangeID = 0;
            User2.ExchangeID = 0;
            Game.World.Exchanges.TryRemove(ID, out Exchange exOut);
            exOut = null;
            IsActive = false;
        }
    }
}
