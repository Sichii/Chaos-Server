using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal delegate bool PursuitDelegate(Client client, Server server, object args);
    internal sealed class Dialogs
    {
        internal Dialog this[ushort dialogId] => DialogList[dialogId];
        private Dictionary<PursuitIds, PursuitDelegate> PursuitDelegates { get; }
        private Dictionary<ushort, Dialog> DialogList { get; }

        internal Dialogs()
        {
            //these will use pursuit id to get the effect
            PursuitDelegates = new Dictionary<PursuitIds, PursuitDelegate>()
            {
                { PursuitIds.Revive, new PursuitDelegate(Revive) },
                { PursuitIds.Teleport, new PursuitDelegate(Teleport) },
                { PursuitIds.Summon, new PursuitDelegate(Summon) },
                { PursuitIds.SummonAll, new PursuitDelegate(SummonAll) },
                { PursuitIds.KillUser, new PursuitDelegate(Kill) },
                { PursuitIds.LouresCitizenship, new PursuitDelegate(LouresCitizenship) },
                { PursuitIds.ReviveUser, new PursuitDelegate(ReviveUser) },
            };

            //these will use dialog id to get the dialog
            DialogList = new Dictionary<ushort, Dialog>()
            {
                { 0, CloseDialog(0) }, //close dialog, activate
                { 1, ItemMerchantMenuDialog( 0, 1, "What would you like to do?",
                    new List<KeyValuePair<string, ushort>>()
                    {
                        new KeyValuePair<string, ushort>("Teleport", 2),
                        new KeyValuePair<string, ushort>("Summon", 3),
                        new KeyValuePair<string, ushort>("Summon All", 4),
                        new KeyValuePair<string, ushort>("Kill User", 5),
                        new KeyValuePair<string, ushort>("Revive Yourself", 8),
                        new KeyValuePair<string, ushort>("Revive User", 9),
                     }
                    )
                },
                { 2, TextEntryDialog(2, 2,
                    "To where? \"mapid x y\" or \"characterName\"\r" +
                    "MAP LIST:\r" +
                    $@"{string.Join("\r", Game.World.Maps.Select(map => $@"{map.Key.ToString()}: {map.Value.Name}"))} ", 12, 0)
                },
                { 3, TextEntryDialog(3, 3, "Who would you like to summon?", 12, 0) },
                { 4, ItemMerchantMenuDialog(4, 4, "This will summon everyone, are you sure?",
                    new List<KeyValuePair<string, ushort>>()
                    {
                        new KeyValuePair<string, ushort>("Yes", 0),
                        new KeyValuePair<string, ushort>("No", ushort.MaxValue),
                    })
                },
                { 5, TextEntryDialog(5, 5, "Who would you like to kill?", 12, 0) },
                { 6, ItemMerchantMenuDialog(7, 6, "Would you like to be a citizen?",
                    new List<KeyValuePair<string, ushort>>()
                    {
                        new KeyValuePair<string, ushort>("Yes", 0),
                        new KeyValuePair<string, ushort>("No", ushort.MaxValue),
                    })
                },
                { 7, NormalDialog(8, 7, false, false, "Need a room, do you? Please, help yourself upstairs.", 0) },
                { ushort.MaxValue, null }, //close dialog and not activate
                { 8, CloseDialog(1) },
                { 9, TextEntryDialog(8, 9, "Who would you like to revive?", 12, 0) }
        };

    }

    /// <summary>
    /// Empty constructor is used for closing dialog
    /// </summary>
    /// <param name="dialogType"></param>
    internal Dialog CloseDialog(ushort pursuitId) => new Dialog(DialogType.CloseDialog, pursuitId, 0, false, false, "", null, 0, null, 0);

    /// <summary>
    /// Creates a standard dialog with or without buttons
    /// </summary>
    /// <param name="nextDialogId">The dialog that comes after this one if you press next.</param>
    internal Dialog NormalDialog(ushort pursuitId, ushort dialogId, bool prevBtn, bool nextBtn, string message, ushort nextDialogId = 0) =>
        new Dialog(DialogType.Normal, pursuitId, dialogId, prevBtn, nextBtn, message, null, nextDialogId, null, 0);

    /// <summary>
    /// Creates a dialog that has a menu on it that leads to other dialogs or effects
    /// </summary>
    /// <param name="options">List of dialog options, and the dialog id they each lead to.</param>
    internal Dialog ItemMerchantMenuDialog(ushort pursuitId, ushort dialogId, string message, List<KeyValuePair<string, ushort>> options) =>
        new Dialog(DialogType.ItemMenu, pursuitId, dialogId, false, false, message, options, 0, null, 0);

    /// <summary>
    /// Creates a dialog that has a menu on it that leads to other dialogs or effects
    /// </summary>
    /// <param name="options">List of dialog options, and the dialog id they each lead to.</param>
    internal Dialog UserMenuDialog(ushort pursuitId, ushort dialogId, string message, List<KeyValuePair<string, ushort>> options) =>
        new Dialog(DialogType.CreatureMenu, pursuitId, dialogId, false, false, message, options, 0, null, 0);

    /// <summary>
    /// Creates a dialog that allows text entry
    /// </summary>
    /// <param name="maxCharacters">Maximum number of input characters to accept.</param>
    /// <param name="nextDialogId">Dialog id that comes after this dialog.</param>
    internal Dialog TextEntryDialog(ushort pursuitId, ushort dialogId, string message, ushort maxCharacters, ushort nextDialogId) =>
        new Dialog(DialogType.TextEntry, pursuitId, dialogId, false, false, message, null, nextDialogId, null, maxCharacters);

    /// <summary>
    /// Returns the effect delegate for the given pursuit ID
    /// </summary>
    internal PursuitDelegate ActivateEffect(PursuitIds pid) => PursuitDelegates[pid];

    /// <summary>
    /// Retreives the previous dialog from the one given
    /// </summary>
    internal Dialog PreviousDialog(Dialog dialog) => DialogList.Values.FirstOrDefault(d => d.NextDialogId == dialog.Id || d.Options.Any(kvp => kvp.Value == dialog.Id));
    /// <summary>
    /// Retreives the next dialog from the one given
    /// </summary>
    internal Dialog NextDialog(ushort dialogId) => DialogList[dialogId];

    #region DialogEffects
    internal bool Revive(Client client, Server server, object args)
    {
        if (!client.User.IsAlive)
        {
            client.User.Attributes.CurrentHP = client.User.Attributes.MaximumHP;
            client.User.Attributes.CurrentMP = client.User.Attributes.MaximumMP;
            client.User.IsAlive = true;
            Game.World.Refresh(client, true);
        }

        return true;
    }
    internal bool Teleport(Client client, Server server, object args)
    {
        string input = (string)args;
        Location warpLoc = new Location();
        User user;

        if (Location.TryParse(input, out warpLoc))
            Game.World.WarpUser(client.User, new Warp(client.User.Location, warpLoc));
        else if (server.TryGetUser(input, out user))
            Game.World.WarpUser(client.User, new Warp(client.User.Location, user.Location));
        else
            client.SendServerMessage(ServerMessageType.Whisper, @"Invalid format. ""mapId xCord yCord"" or ""characterName""");
        return true;
    }

    internal bool Summon(Client client, Server server, object args)
    {
        string input = (string)args;
        User user;

        if (server.TryGetUser(input, out user))
            Game.World.WarpUser(user, new Warp(user.Location, client.User.Location));
        else
            client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");

        return true;
    }

    internal bool SummonAll(Client client, Server server, object args)
    {
        List<User> allUsers = server.WorldClients.Select(c => c.User).ToList();
        allUsers.Remove(client.User);

        foreach (User user in allUsers)
            Game.World.WarpUser(user, new Warp(user.Location, client.User.Location));

        return true;
    }

    internal bool Kill(Client client, Server server, object args)
    {
        string input = (string)args;
        User user;

        if (server.TryGetUser(input, out user))
            Game.Extensions.KillUser(user.Client, user);
        else
            client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");

        return true;
    }

    internal bool LouresCitizenship(Client client, Server server, object args)
    {
        client.User.Nation = Nation.Loures;

        return true;
    }
    internal bool ReviveUser(Client client, Server server, object args)
    {
        string input = (string)args;
        User user;

        if (server.TryGetUser(input, out user))
            Game.Extensions.ReviveUser(user.Client, user);
        else
            client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");
        return true;
    }
        #endregion
    }
}