using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.Cryptography;
using Chaos.DataObjects;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Networking.Model.Client;
using Chaos.Options;
using Chaos.Packets;
using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;
using Chaos.Servers.Interfaces;
using Chaos.WorldObjects;
using Chaos.WorldObjects.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Servers;

public class WorldServer : ServerBase, IWorldServer
{
    private readonly IClientFactory<IWorldClient> ClientFactory;
    private readonly ICacheManager<string, Metafile> MetafileManager;
    private readonly ISaveManager<User> UserSaveManager;
    public ConcurrentDictionary<uint, IWorldClient> Clients { get; }
    protected new WorldClientHandler?[] ClientHandlers { get; }
    protected override WorldOptions Options { get; }

    public IEnumerable<User> Users => Clients
        .Select(kvp => kvp.Value.User)
        .Where(user => user != null!);

    public WorldServer(
        IClientFactory<IWorldClient> clientFactory,
        ICacheManager<string, Metafile> metafileManager,
        ISaveManager<User> userSaveManager,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptionsSnapshot<WorldOptions> options,
        ILogger<WorldServer> logger)
        : base(redirectManager, packetSerializer, options, logger)
    {
        ClientFactory = clientFactory;
        MetafileManager = metafileManager;
        UserSaveManager = userSaveManager;
        Options = options.Value;
        Clients = new ConcurrentDictionary<uint, IWorldClient>();
        ClientHandlers = new WorldClientHandler[byte.MaxValue];
        
        IndexHandlers();
    }

    #region Connection / Handler
    protected delegate ValueTask WorldClientHandler(IWorldClient client, ref ClientPacket packet);
    
    public override ValueTask HandlePacketAsync<TClient>(TClient client, ref ClientPacket packet)
    {
        if (client is IWorldClient worldClient)
        {
            var handler = ClientHandlers[(byte)packet.OpCode];

            return handler?.Invoke(worldClient, ref packet) ?? default;
        }

        return base.HandlePacketAsync(client, ref packet);
    }

    protected sealed override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;
        
        base.IndexHandlers();
        var oldHandlers = base.ClientHandlers;

        for (var i = 0; i < byte.MaxValue; i++)
        {
            var old = oldHandlers[i];

            if (old == null)
                continue;

            ClientHandlers[i] = new WorldClientHandler(old);
        }
        
        //ClientHandlers[(byte)ClientOpCode.] =
        ClientHandlers[(byte)ClientOpCode.RequestMapData] = OnMapDataRequest;
        ClientHandlers[(byte)ClientOpCode.ClientWalk] = OnClientWalk;
        ClientHandlers[(byte)ClientOpCode.Pickup] = OnPickup;
        ClientHandlers[(byte)ClientOpCode.ItemDrop] = OnItemDropped;
        ClientHandlers[(byte)ClientOpCode.ExitRequest] = OnExitRequest;
        //ClientHandlers[(byte)ClientOpCode.DisplayObjectRequest] =
        ClientHandlers[(byte)ClientOpCode.Ignore] = OnIgnore;
        ClientHandlers[(byte)ClientOpCode.PublicMessage] = OnPublicMessage;
        ClientHandlers[(byte)ClientOpCode.SpellUse] = OnUseSpell;
        ClientHandlers[(byte)ClientOpCode.ClientRedirected] = OnClientRedirected;
        ClientHandlers[(byte)ClientOpCode.Turn] = OnTurn;
        ClientHandlers[(byte)ClientOpCode.SpaceBar] = OnSpacebar;
        ClientHandlers[(byte)ClientOpCode.RequestWorldList] = OnWorldListRequest;
        ClientHandlers[(byte)ClientOpCode.Whisper] = OnWhisper;
        ClientHandlers[(byte)ClientOpCode.UserOptionToggle] = OnUserOptionToggle;
        ClientHandlers[(byte)ClientOpCode.ItemUse] = OnUseItem;
        ClientHandlers[(byte)ClientOpCode.Emote] = OnEmote;
        ClientHandlers[(byte)ClientOpCode.GoldDrop] = OnGoldDropped;
        ClientHandlers[(byte)ClientOpCode.ItemDroppedOnCreature] = OnItemDroppedOnCreature;
        ClientHandlers[(byte)ClientOpCode.GoldDroppedOnCreature] = OnGoldDroppedOnCreature;
        ClientHandlers[(byte)ClientOpCode.RequestProfile] = OnProfileRequest;
        ClientHandlers[(byte)ClientOpCode.GroupRequest] = OnGroupRequest;
        ClientHandlers[(byte)ClientOpCode.ToggleGroup] = OnToggleGroup;
        ClientHandlers[(byte)ClientOpCode.SwapSlot] = OnSwapSlot;
        ClientHandlers[(byte)ClientOpCode.RequestRefresh] = OnRefreshRequest;
        ClientHandlers[(byte)ClientOpCode.PursuitRequest] = OnPursuitRequest;
        ClientHandlers[(byte)ClientOpCode.DialogResponse] = OnDialogResponse;
        ClientHandlers[(byte)ClientOpCode.BoardRequest] = OnBoardRequest;
        ClientHandlers[(byte)ClientOpCode.SkillUse] = OnUseSkill;
        ClientHandlers[(byte)ClientOpCode.WorldMapClick] = OnWorldMapClick;
        ClientHandlers[(byte)ClientOpCode.Click] = OnClick;
        ClientHandlers[(byte)ClientOpCode.Unequip] = OnUnequip;
        ClientHandlers[(byte)ClientOpCode.RaiseStat] = OnRaiseStat;
        ClientHandlers[(byte)ClientOpCode.Exchange] = OnExchange;
        ClientHandlers[(byte)ClientOpCode.BeginChant] = OnBeginChant;
        ClientHandlers[(byte)ClientOpCode.Chant] = OnChant;
        ClientHandlers[(byte)ClientOpCode.Profile] = OnProfile;
        ClientHandlers[(byte)ClientOpCode.SocialStatus] = OnSocialStatus; 
        ClientHandlers[(byte)ClientOpCode.MetafileRequest] = OnMetafileRequest;
    }
    
    protected override void OnConnection(IAsyncResult ar)
    {
        var serverSocket = (Socket)ar.AsyncState!;
        var clientSocket = serverSocket.EndAccept(ar);

        serverSocket.BeginAccept(OnConnection, serverSocket);

        var client = ClientFactory.CreateClient(clientSocket);

        if (!Clients.TryAdd(client.Id, client))
        {
            Logger.LogError("Somehow two clients got the same id. (Id: {Id})", client.Id);
            client.Disconnect();

            return;
        }

        client.OnDisconnected += (sender, args) =>
        {
            var sClient = (IWorldClient)sender!;
            Clients.TryRemove(sClient.Id, out _);
        };
        client.BeginReceive();
    }
    #endregion
    
    public ValueTask OnBeginChant(IWorldClient client, ref ClientPacket clientPacket)
    {
        if (!client.User.Status.HasFlag(Status.Dead))
            client.User.UserState |= UserState.IsChanting;
        else
            client.SendCancelCasting();

        return default;
    }

    public ValueTask OnBoardRequest(IWorldClient client, ref ClientPacket clientPacket) => default;

    public ValueTask OnClick(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var targetId, var targetPoint) = PacketSerializer.Deserialize<ClickArgs>(ref clientPacket);

        if (targetId.HasValue)
            client.User.MapInstance.Click(targetId.Value, client.User);
        else if (targetPoint.HasValue)
            client.User.MapInstance.Click(targetPoint.Value, client.User);

        return default;
    }

    public ValueTask OnClientRedirected(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClientRedirectedArgs>(ref clientPacket);

        if (!RedirectManager.TryGetRemove(args.Id, out var redirect))
        {
            client.Disconnect();

            return default;
        }

        var existingUser = Users.FirstOrDefault(user => user.Name.EqualsI(redirect.Name));

        //double logon, disconnect both clients
        if (existingUser != null)
        {
            existingUser.Client.Disconnect();
            client.Disconnect();

            return default;
        }

        return OnClientRedirectedAsync(client, args, redirect);
    }

    public async ValueTask OnClientRedirectedAsync(IWorldClient client, ClientRedirectedArgs args, Redirect redirect)
    {
        client.CryptoClient = new CryptoClient(args.Seed, args.Key, args.Name);
        var user = await UserSaveManager.LoadAsync(client, redirect.Name);
        client.User = user;
        
        client.SendAttributes(StatUpdateType.Full);
        client.SendLightLevel(LightLevel.Lightest);
        client.SendUserId();
        user.MapInstance.AddObject(user, user.Point);
        client.SendProfileRequest();
    }

    public ValueTask OnClientWalk(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClientWalkArgs>(ref clientPacket);

        client.User.MapInstance.ClientWalk(client.User, args.Direction);

        return default;
    }

    public ValueTask OnDialogResponse(IWorldClient client, ref ClientPacket clientPacket) => throw new NotImplementedException();

    public ValueTask OnChant(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<DisplayChantArgs>(ref clientPacket);

        client.User.MapInstance.ShowPublicMessage(PublicMessageType.Chant, args.ChantMessage, client.User);

        return default;
    }

    public ValueTask OnEmote(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<EmoteArgs>(ref clientPacket);

        if ((int)args.BodyAnimation <= 44)
            client.User.MapInstance.ShowBodyAnimation(args.BodyAnimation, client.User);

        return default;
    }

    public ValueTask OnExchange(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ExchangeArgs>(ref clientPacket);
        //TODO: implement this shit

        return default;
    }

    public ValueTask OnExitRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ExitRequestArgs>(ref clientPacket);
        
        if(args.IsRequest)
            client.SendConfirmExit();
        else
        {
            var redirect = new Redirect(client.CryptoClient, Options.LoginRedirect, ServerType.Login);
            RedirectManager.Add(redirect);
            client.SendRedirect(redirect);
        }

        return default;
    }

    public ValueTask OnGoldDropped(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var amount, var destinationPoint) = PacketSerializer.Deserialize<GoldDropArgs>(ref clientPacket);
        var map = client.User.MapInstance;
        
        if (amount <= 0)
            return default;

        using var monitor = map.AutoMonitor.Enter();

        if (!client.User.WithinRange(destinationPoint, Options.DropRange))
            return default;
        
        if (map.IsWall(destinationPoint))
            return default;
        
        var currentGold = client.User.Gold;

        if (currentGold < amount)
            return default;

        currentGold -= amount;
        client.User.Gold = currentGold;

        client.SendAttributes(StatUpdateType.ExpGold);

        var money = new Money(amount);
        map.AddObject(money, destinationPoint);

        return default;
    }

    public ValueTask OnGoldDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var amount, var targetId) = PacketSerializer.Deserialize<GoldDroppedOnCreatureArgs>(ref clientPacket);
        var map = client.User.MapInstance;
        
        if (amount <= 0)
            return default;

        if (client.User.Gold < amount)
            return default;
        
        using var monitor = map.AutoMonitor.Enter();

        if (!map.TryGetObject<Creature>(targetId, out var target))
            return default;

        if (!client.User.WithinRange(target, Options.TradeRange))
            return default;
        
        target.GoldDroppedOn(amount, client.User);
        
        return default;
    }

    public ValueTask OnGroupRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<GroupRequestArgs>(ref clientPacket);
        //TODO: implement this

        return default;
    }

    public ValueTask OnIgnore(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var ignoreType, var targetName) = PacketSerializer.Deserialize<IgnoreArgs>(ref clientPacket);

        switch (ignoreType)
        {
            case IgnoreType.Request:
                client.SendServerMessage(ServerMessageType.ScrollWindow, client.User.IgnoreList.ToString());
                break;
            case IgnoreType.AddUser:
                if (!string.IsNullOrEmpty(targetName))
                    client.User.IgnoreList.Add(targetName);
                break;
            case IgnoreType.RemoveUser:
                if (!string.IsNullOrEmpty(targetName))
                    client.User.IgnoreList.Remove(targetName);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnItemDropped(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var destinationPoint, var count) = PacketSerializer.Deserialize<ItemDropArgs>(ref clientPacket);

        var map = client.User.MapInstance;
        
        using var monitor = map.AutoMonitor.Enter();
        
        if (map.IsWall(destinationPoint))
            return default;

        if (!client.User.WithinRange(destinationPoint, Options.DropRange))
            return default;

        if (!client.User.Inventory.RemoveQuantity(sourceSlot, count, out var item))
            return default;

        map.AddObject(item.ToGroundItem(), destinationPoint);
        
        return default;
    }

    public ValueTask OnItemDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var targetId, var count) = PacketSerializer.Deserialize<ItemDroppedOnCreatureArgs>(ref clientPacket);

        var map = client.User.MapInstance;

        using var monitor = map.AutoMonitor.Enter();
        
        if (!map.TryGetObject<Creature>(targetId, out var target))
            return default;

        if (!client.User.WithinRange(target, Options.TradeRange))
            return default;

        if (!client.User.Inventory.TryGetObject(sourceSlot, out var item))
            return default;

        if (item == null)
            return default;

        if (item.Count < count)
            return default;

        target.ItemDroppedOn(sourceSlot, count, client.User);

        return default;
    }

    public ValueTask OnMapDataRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.SendMapData();

        return default;
    }

    public ValueTask OnMetafileRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var metafileRequestType, var name) = PacketSerializer.Deserialize<MetafileRequestArgs>(ref clientPacket);

        switch (metafileRequestType)
        {
            case MetafileRequestType.DataByName:
                client.SendMetafile(MetafileRequestType.DataByName, MetafileManager, name);
                break;
            case MetafileRequestType.AllCheckSums:
                client.SendMetafile(MetafileRequestType.AllCheckSums, MetafileManager);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnPickup(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var destinationSlot, var sourcePoint) = PacketSerializer.Deserialize<PickupArgs>(ref clientPacket);

        var map = client.User.MapInstance;
        
        using var monitor = map.AutoMonitor.Enter();

        if (!client.User.WithinRange(sourcePoint, Options.PickupRange))
            return default;

        var obj = map.ObjectsAtPoint<GroundItem>(sourcePoint).FirstOrDefault();

        if (obj == null)
            return default;

        if (!client.User.CanCarry(obj.Item))
            return default;

        if (client.User.Inventory.TryAdd(destinationSlot, obj.Item))
            map.RemoveObject(obj);

        return default;
    }

    public ValueTask OnProfile(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var portraitData, var profileMessage) = PacketSerializer.Deserialize<ProfileArgs>(ref clientPacket);

        client.User.Portrait = portraitData;
        client.User.ProfileMessage = profileMessage;

        return default;
    }

    public ValueTask OnProfileRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.SendSelfProfile();

        return default;
    }

    public ValueTask OnPublicMessage(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var publicMessageType, var message) = PacketSerializer.Deserialize<PublicMessageArgs>(ref clientPacket);
        
        client.User.MapInstance.ShowPublicMessage(publicMessageType, $"{client.User.Name}: {message}", client.User);

        return default;
    }

    public ValueTask OnPursuitRequest(IWorldClient client, ref ClientPacket clientPacket) => throw new NotImplementedException();

    public ValueTask OnRaiseStat(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<RaiseStatArgs>(ref clientPacket);

        if (client.User.StatSheet.UnspentPoints > 0)
        {
            switch(args.Stat)
            {
                case Stat.STR:
                    client.User.StatSheet.Str++;
                    break;
                case Stat.DEX:
                    client.User.StatSheet.Dex++;
                    break;
                case Stat.INT:
                    client.User.StatSheet.Int++;
                    break;
                case Stat.WIS:
                    client.User.StatSheet.Wis++;
                    break;
                case Stat.CON:
                    client.User.StatSheet.Con++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            client.User.StatSheet.UnspentPoints--;
        }

        return default;
    }

    public ValueTask OnRefreshRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.User.MapInstance.Refresh(client.User);

        return default;
    }

    public ValueTask OnSocialStatus(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SocialStatusArgs>(ref clientPacket);

        client.User.SocialStatus = args.SocialStatus;

        return default;
    }

    public ValueTask OnSpacebar(IWorldClient client, ref ClientPacket clientPacket)
    {
        //TODO: assails i guess

        return default;
    }

    public ValueTask OnSwapSlot(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var panelType, var slot1, var slot2) = PacketSerializer.Deserialize<SwapSlotArgs>(ref clientPacket);

        switch(panelType)
        {
            case PanelType.Inventory:
                client.User.Inventory.TrySwap(slot1, slot2);
                break;
            case PanelType.SpellBook:
                client.User.SpellBook.TrySwap(slot1, slot2);
                break;
            case PanelType.SkillBook:
                client.User.SkillBook.TrySwap(slot1, slot2);
                break;
            case PanelType.Equipment:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return default;
    }

    public ValueTask OnToggleGroup(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.User.Options.Toggle(UserOption.Group);

        if (client.User.Group != null)
            client.User.Group?.Leave(client.User);
        else
            client.SendSelfProfile();

        return default;
    }

    public ValueTask OnTurn(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<TurnArgs>(ref clientPacket);
        
        client.User.MapInstance.ShowTurn(args.Direction, client.User);

        return default;
    }

    public ValueTask OnUnequip(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UnequipArgs>(ref clientPacket);

        if (client.User.Inventory.IsFull)
            return default;

        if (!client.User.Equipment.TryGetRemove((byte)args.EquipmentSlot, out var item))
            return default;

        if (item == null)
            return default;

        client.User.Inventory.TryAddToNextSlot(item);

        return default;
    }

    public ValueTask OnUseItem(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ItemUseArgs>(ref clientPacket);
        //TODO: probably activate a script

        return default;
    }

    public ValueTask OnUserOptionToggle(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UserOptionToggleArgs>(ref clientPacket);

        if (args.UserOption == UserOption.Request)
        {
            client.SendServerMessage(ServerMessageType.UserOptions, client.User.Options.ToString());

            return default;
        }

        client.User.Options.Toggle(args.UserOption);

        return default;
    }

    public ValueTask OnUseSkill(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SkillUseArgs>(ref clientPacket);
        
        //TODO: probably activate a script

        return default;
    }

    public ValueTask OnUseSpell(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SpellUseArgs>(ref clientPacket);
        
        //TODO: probably activate a script

        return default;
    }

    public ValueTask OnWhisper(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var targetName, var message) = PacketSerializer.Deserialize<WhisperArgs>(ref clientPacket);

        var targetUser = Users.FirstOrDefault(user => user.Name.EqualsI(targetName));

        if (message.Length > 100)
            return default;
        
        if (targetUser == null)
        {
            client.SendServerMessage(ServerMessageType.ActiveMessage, $"{targetName} is not online");
            return default;
        }

        if (targetUser.Equals(client.User))
        {
            client.SendServerMessage(ServerMessageType.Whisper, "Talking to yourself?");
            
            return default;
        }
        
        if (targetUser.SocialStatus == SocialStatus.DoNotDisturb)
        {
            client.SendServerMessage(ServerMessageType.Whisper, $"{targetUser.Name} doesn't want to be bothered");

            return default;
        }

        //if someone is being ignored, they shouldnt know it
        //let them waste their time typing for no reason
        if (targetUser.IgnoreList.ContainsI(client.User.Name))
        {
            Logger.LogInformation("Ignored by: {TargetName}, From: {FromName}, Message: {Message}",
                targetUser.Name,
                client.User.Name,
                message);

            client.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");

            return default;
        }

        client.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");
        targetUser.Client.SendServerMessage(ServerMessageType.Whisper, $"[{client.User.Name}] < {message}");
        
        return default;
    }

    public ValueTask OnWorldListRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.SendWorldList(Users.ToList());

        return default;
    }

    public ValueTask OnWorldMapClick(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<WorldMapClickArgs>(ref clientPacket);
        
        //TODO: world maps dont exist yet

        return default;
    }
}