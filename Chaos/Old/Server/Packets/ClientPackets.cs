/*
internal class ClientPackets
{

    internal static ImmutableArray<Handler> Handlers
    {
        get
        {
            var cp = new ClientPackets();
            
            var handles = new Handler[byte.MaxValue];
            handles[(byte)ClientOpCode.RequestConnectionInfo] = new Handler(cp.RequestConnectionInfo);
            handles[(byte)ClientOpCode.CreateChar1] = new Handler(cp.CreateChar1);
            handles[(byte)ClientOpCode.Login] = new Handler(cp.Login);
            handles[(byte)ClientOpCode.CreateChar2] = new Handler(cp.CreateChar2);
            handles[(byte)ClientOpCode.RequestMapData] = new Handler(cp.RequestMapData);
            handles[(byte)ClientOpCode.ClientWalk] = new Handler(cp.ClientWalk);
            handles[(byte)ClientOpCode.Pickup] = new Handler(cp.Pickup);
            handles[(byte)ClientOpCode.ItemDrop] = new Handler(cp.Drop);
            handles[(byte)ClientOpCode.ExitRequest] = new Handler(cp.ExitClient);
            handles[(byte)ClientOpCode.Ignore] = new Handler(cp.Ignore);
            handles[(byte)ClientOpCode.PublicMessage] = new Handler(cp.PublicChat);
            handles[(byte)ClientOpCode.SpellUse] = new Handler(cp.UseSpell);
            handles[(byte)ClientOpCode.ClientRedirected] = new Handler(cp.JoinClient);
            handles[(byte)ClientOpCode.Turn] = new Handler(cp.Turn);
            handles[(byte)ClientOpCode.SpaceBar] = new Handler(cp.SpaceBar);
            handles[(byte)ClientOpCode.RequestWorldList] = new Handler(cp.RequestWorldList);
            handles[(byte)ClientOpCode.Whisper] = new Handler(cp.Whisper);
            handles[(byte)ClientOpCode.UserOptionToggle] = new Handler(cp.ToggleUserOption);
            handles[(byte)ClientOpCode.ItemUse] = new Handler(cp.UseItem);
            handles[(byte)ClientOpCode.Emote] = new Handler(cp.Emote);
            handles[(byte)ClientOpCode.GoldDrop] = new Handler(cp.DropGold);
            handles[(byte)ClientOpCode.PasswordChange] = new Handler(cp.ChangePassword);
            handles[(byte)ClientOpCode.ItemDRoppedOnCreature] = new Handler(cp.DropItemOnCreature);
            handles[(byte)ClientOpCode.GoldDroppedOnEcreature] = new Handler(cp.DropGoldOnCreature);
            handles[(byte)ClientOpCode.RequestProfile] = new Handler(cp.RequestProfile);
            handles[(byte)ClientOpCode.GroupRequest] = new Handler(cp.RequestGroup);
            handles[(byte)ClientOpCode.ToggleGroup] = new Handler(cp.ToggleGroup);
            handles[(byte)ClientOpCode.SwapSlot] = new Handler(cp.SwapSlot);
            handles[(byte)ClientOpCode.RequestRefresh] = new Handler(cp.RequestRefresh);
            handles[(byte)ClientOpCode.PursuitRequest] = new Handler(cp.RequestPursuit);
            handles[(byte)ClientOpCode.DialogResponse] = new Handler(cp.ReplyDialog);
            handles[(byte)ClientOpCode.BoardRequest] = new Handler(cp.Board);
            handles[(byte)ClientOpCode.SkillUse] = new Handler(cp.UseSkill);
            handles[(byte)ClientOpCode.WorldMapClick] = new Handler(cp.ClickWorldMap);
            handles[(byte)ClientOpCode.Click] = new Handler(cp.ClickObject);
            handles[(byte)ClientOpCode.Unequip] = new Handler(cp.RemoveEquipment);
            handles[(byte)ClientOpCode.HeartBeat] = new Handler(cp.KeepAlive);
            handles[(byte)ClientOpCode.RaiseStat] = new Handler(cp.ChangeStat);
            handles[(byte)ClientOpCode.Exchange] = new Handler(cp.Exchange);
            handles[(byte)ClientOpCode.RequestLoginNotification] = new Handler(cp.RequestLoginNotification);
            handles[(byte)ClientOpCode.BeginChant] = new Handler(cp.BeginChant);
            handles[(byte)ClientOpCode.Chant] = new Handler(cp.DisplayChant);
            handles[(byte)ClientOpCode.Profile] = new Handler(cp.Personal);
            handles[(byte)ClientOpCode.ServerTableRequest] = new Handler(cp.RequestServerTable);
            handles[(byte)ClientOpCode.ChangeSequence] = new Handler(cp.ChangeSequence);
            handles[(byte)ClientOpCode.RequestHomepage] = new Handler(cp.RequestHomepage);
            handles[(byte)ClientOpCode.SynchronizeTicks] = new Handler(cp.SynchronizeTicks);
            handles[(byte)ClientOpCode.SocialStatus] = new Handler(cp.ChangeSocialStatus);
            handles[(byte)ClientOpCode.MetafileRequest] = new Handler(cp.RequestMetaFile);

            return handles.ToImmutableArray();
        }
    }

    internal ClientPackets() { }

    private void RequestConnectionInfo(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}]", client);
        Game.Game.RequestConnectionInfo(client);
    }

    private void CreateChar1(Client.Client client, ClientPacket packet)
    {
        var name = packet.ReadString8();
        var pw = packet.ReadString8();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] NAME: {name} | PASSWORD: {pw}", client);
        Game.Game.CreateChar1(client, name, pw);
    }

    private void Login(Client.Client client, ClientPacket packet)
    {
        var name = packet.ReadString8();
        var pw = packet.ReadString8();
        packet.ReadByte();
        packet.ReadByte();
        packet.ReadUInt32();
        packet.ReadUInt16();
        packet.ReadUInt32();
        packet.ReadUInt16();
        packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] NAME: {name} | PASSWORD: {pw}", client);
        Game.Game.Login(client, name, pw);
    }

    private void CreateChar2(Client.Client client, ClientPacket packet)
    {
        var hairStyle = packet.ReadByte();
        var gender = (Gender)packet.ReadByte();
        var hairColor = packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] HAIR: {hairStyle} | GENDER: {gender} | COLOR: {hairColor}", client);
        Game.Game.CreateChar2(client, hairStyle, gender, hairColor);
    }

    private void RequestMapData(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.RequestMapData(client);
    }
    private void ClientWalk(Client.Client client, ClientPacket packet)
    {
        var direction = (Direction)packet.ReadByte();
        int stepCount = packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] DIRECTION: {direction} | STEPS: {stepCount}", client);
        Game.Game.ClientWalk(client, direction, stepCount);
    }
    private void Pickup(Client.Client client, ClientPacket packet)
    {
        var slot = packet.ReadByte();
        var groundPoint = packet.ReadPoint();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] DESTINATION_SLOT: {slot} | SOURCE_POINT: {groundPoint}", client);
        Game.Game.Pickup(client, slot, groundPoint);
    }
    private void Drop(Client.Client client, ClientPacket packet)
    {
        var slot = packet.ReadByte();
        var groundPoint = packet.ReadPoint();
        var count = packet.ReadUInt32();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SOURCE_SLOT: {slot} | DESTINATION_POINT: {groundPoint} | COUNT: {count}", client);
        Game.Game.Drop(client, slot, groundPoint, count);
    }
    private void ExitClient(Client.Client client, ClientPacket packet)
    {
        var requestExit = false;

        if (packet.Position != packet.Length)
            requestExit = packet.ReadBoolean();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] EXIT: {requestExit}", client);
        Game.Game.ExitClient(client, requestExit);
    }
    private void Ignore(Client.Client client, ClientPacket packet)
    {
        var type = (IgnoreType)packet.ReadByte();
        string targetName = null;

        if (type != IgnoreType.Request)
            targetName = packet.ReadString8();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | TARGET: {targetName ?? "n/a"}", client);
        Game.Game.Ignore(client, type, targetName);
    }
    private void PublicChat(Client.Client client, ClientPacket packet)
    {
        var type = (PublicMessageType)packet.ReadByte();
        var message = packet.ReadString8();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | MESSAGE: {message}", client);
        Game.Game.PublicChat(client, type, message);
    }
    private void UseSpell(Client.Client client, ClientPacket packet)
    {
        var slot = packet.ReadByte();
        var targetId = client.User.ID;
        var targetPoint = client.User.Point;
        var prompt = "";

        if(client.User.SpellBook[slot]?.SpellType == SpellType.Prompt)
            prompt = packet.ReadString();
        else if (packet.Position != packet.Length)
        {
            targetId = packet.ReadInt32();
            targetPoint = packet.ReadPoint();
        }

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SOURCE_SLOT: {slot} | TARGET_ID: {targetId} | TARGET_POINT: {targetPoint}", client);
        Game.Game.UseSpell(client, slot, targetId, targetPoint, prompt);
    }
    private void JoinClient(Client.Client client, ClientPacket packet)
    {
        var seed = packet.ReadByte();
        var key = packet.ReadString8();
        var name = packet.ReadString8();
        var id = packet.ReadUInt32();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SEED: {seed} | KEY: {key} | NAME: {name} | ID: {id}", client);
        var redirect = client.Server.Redirects.FirstOrDefault(r => r.Id == id);

        if (redirect != null)
        {
            client.ServerType = redirect.Type;
            client.Server.Redirects.Remove(redirect);
            Game.Game.JoinClient(client, seed, Encoding.ASCII.GetBytes(key), name, id);
        }
        else
            client.Disconnect();
    }
    private void Turn(Client.Client client, ClientPacket packet)
    {
        var direction = (Direction)packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] DIRECTION: {direction}", client);
        Game.Game.Turn(client, direction);
    }

    private void SpaceBar(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.SpaceBar(client);
    }
    private void RequestWorldList(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.RequestWorldList(client);
    }
    private void Whisper(Client.Client client, ClientPacket packet)
    {
        var targetName = packet.ReadString8();
        var message = packet.ReadString8();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TARGET: {targetName} | MESSAGE: {message}", client);
        Game.Game.Whisper(client, targetName, message);
    }
    private void ToggleUserOption(Client.Client client, ClientPacket packet)
    {
        var option = (UserOption)packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] OPTION: {option}", client);
        Game.Game.ToggleUserOption(client, option);
    }
    private void UseItem(Client.Client client, ClientPacket packet)
    {
        var slot = packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SOURCE_SLOT: {slot}", client);
        Game.Game.UseItem(client, slot);
    }
    private void Emote(Client.Client client, ClientPacket packet)
    {
        var animNum = packet.ReadByte();
        var anim = (BodyAnimation)(animNum + 9);

        if (animNum > 35) return;

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] EMOTE: {anim}", client);
        Game.Game.AnimateCreature(client, anim);
    }
    private void DropGold(Client.Client client, ClientPacket packet)
    {
        var amount = packet.ReadUInt32();
        var groundPoint = packet.ReadPoint();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] AMOUNT: {amount} | DESTINATION_POINT: {groundPoint}", client);
        Game.Game.DropGold(client, amount, groundPoint);
    }
    private void ChangePassword(Client.Client client, ClientPacket packet)
    {
        var name = packet.ReadString8();
        var currentPw = packet.ReadString8();
        var newPw = packet.ReadString8();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] NAME: {name} | CURRENT: {currentPw} | NEW: {newPw}", client);
        Game.Game.ChangePassword(client, name, currentPw, newPw);
    }
    private void DropItemOnCreature(Client.Client client, ClientPacket packet)
    {
        var slot = packet.ReadByte();
        var targetId = packet.ReadInt32();
        var count = packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SOURCE_SLOT: {slot} | TARGET_ID: {targetId} | COUNT: {count}", client);
        Game.Game.DropItemOnCreature(client, slot, targetId, count);
    }
    private void DropGoldOnCreature(Client.Client client, ClientPacket packet)
    {
        var amount = packet.ReadUInt32();
        var targetId = packet.ReadInt32();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] AMOUNT: {amount} | TARGET_ID: {targetId}", client);
        Game.Game.DropGoldOnCreature(client, amount, targetId);
    }
    private void RequestProfile(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.RequestProfile(client);
    }
    private void RequestGroup(Client.Client client, ClientPacket packet)
    {
        GroupBox box = default;
        var type = (GroupRequestType)packet.ReadByte();

        if (type == GroupRequestType.Groupbox)
        {
            var leader = packet.ReadString8();
            var text = packet.ReadString8();
            packet.ReadByte();
            var minLevel = packet.ReadByte();
            var maxLevel = packet.ReadByte();
            var maxOfEach = new byte[6];
            maxOfEach[(byte)BaseClass.Warrior] = packet.ReadByte();
            maxOfEach[(byte)BaseClass.Wizard] = packet.ReadByte();
            maxOfEach[(byte)BaseClass.Rogue] = packet.ReadByte();
            maxOfEach[(byte)BaseClass.Priest] = packet.ReadByte();
            maxOfEach[(byte)BaseClass.Monk] = packet.ReadByte();

            box = new GroupBox(text, maxLevel, maxOfEach);
        }
        var targetName = packet.ReadString8();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | TARGET: {targetName}", client);
        Game.Game.RequestGroup(client, type, targetName, box);
    }
    private void ToggleGroup(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.ToggleGroup(client);
    }
    private void SwapSlot(Client.Client client, ClientPacket packet)
    {
        var panelType = (PanelType)packet.ReadByte();
        var origSlot = packet.ReadByte();
        var endSlot = packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] PANEL: {panelType} | FROM: {origSlot} | TO: {endSlot}", client);
        Game.Game.SwapSlot(client, panelType, origSlot, endSlot);
    }
    private void RequestRefresh(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.RequestRefresh(client);
    }
    private void RequestPursuit(Client.Client client, ClientPacket packet)
    {
        var objType = (GameObjectType)packet.ReadByte();
        var objId = packet.ReadInt32();
        var pid = packet.ReadUInt16();
        var pursuitId = Enum.IsDefined(CONSTANTS.PURSUITIDS_TYPE, pid) ? (PursuitId)pid 
            : PursuitId.None;
        var args = packet.ReadBytes(packet.Length - packet.Position);

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] OBJECT: {objType} | OBJECT_ID: {objId} | PURSUIT: {pursuitId} | ARGS: {args.Length > 0}", client);
        Game.Game.RequestPursuit(client, objType, objId, pursuitId, args);
    }
    private void ReplyDialog(Client.Client client, ClientPacket packet)
    {
        var objType = (GameObjectType)packet.ReadByte();
        var objId = packet.ReadInt32();
        var pid = packet.ReadUInt16();
        var pursuitId = Enum.IsDefined(CONSTANTS.PURSUITIDS_TYPE, pid) ? (PursuitId)pid 
            : PursuitId.None;
        var dialogId = packet.ReadUInt16();

        var argsType = DialogArgsType.None;
        byte opt = 0;
        var input = "";

        if(packet.Position != packet.Length)
        {
            argsType = (DialogArgsType)packet.ReadByte();

            switch (argsType)
            {
                case DialogArgsType.MenuResponse:
                    opt = packet.ReadByte();
                    break;
                case DialogArgsType.TextResponse:
                    input = packet.ReadString8();
                    break;
            }
        }

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] OBJECT: {objType} | OBJECT_ID: {objId} | PURSUIT: {pursuitId} | DIALOG_ID: {dialogId} | ARGS: {argsType}", client);
        Game.Game.ReplyDialog(client, objType, objId, pursuitId, dialogId, argsType, opt, input);
    }

    //this packet is literally retarded
    private void Board(Client.Client client, ClientPacket packet)
    {
        var type = (BoardRequestType)packet.ReadByte();

        switch (type) //request type
        {
            case BoardRequestType.BoardList:
                //Board List
                //client.Enqueue(client.ServerPackets.BulletinBoard);
                break;
            case BoardRequestType.ViewBoard:
            {
                //Post list for boardNum
                var boardNum = packet.ReadUInt16();
                var startPostNum = packet.ReadUInt16(); //you send the newest mail first, which will have the highest number. startPostNum counts down.
                //packet.ReadByte() is always 0xF0(240) ???
                //the client spam requests this like holy fuck, put a timer on this so you only send 1 packet
                break;
            }
            case BoardRequestType.ViewPost:
            {
                //Post
                var boardNum = packet.ReadUInt16();
                var postId = packet.ReadUInt16(); //the post number they want, counting up (what the fuck?)
                //mailbox = boardNum 0
                //otherwise boardnum is the index of the board you're accessing
                switch (packet.ReadSByte()) //board controls
                {
                    case -1: //clicked next for older post
                        break;
                    case 0: //requested a specific post from the post list
                        break;
                    case 1: //clicked previous for newer post
                        break;
                }
                break;
            }
            case BoardRequestType.NewPost: //new post
            {
                var boardNum = packet.ReadUInt16();
                var subject = packet.ReadString8();
                var message = packet.ReadString16();
                break;
            }
            case BoardRequestType.Delete: //delete post
            {
                var boardNum = packet.ReadUInt16();
                var postId = packet.ReadUInt16(); //the post number they want to delete, counting up
                break;
            }

            case BoardRequestType.SendMail: //send mail
            {
                var boardNum = packet.ReadUInt16();
                var targetName = packet.ReadString8();
                var subject = packet.ReadString8();
                var message = packet.ReadString16();
                break;
            }
            case BoardRequestType.Highlight: //highlight message
            {
                var boardNum = packet.ReadUInt16();
                var postId = packet.ReadUInt16();
                break;
            }
        }

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type}", client);
        Game.Game.Boards(client);
    }
    private void UseSkill(Client.Client client, ClientPacket packet)
    {
        var slot = packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SOURCE_SLOT: {slot}", client);
        Game.Game.UseSkill(client, slot);
    }

    private void ClickWorldMap(Client.Client client, ClientPacket packet)
    {
        var nodeCheckSum = packet.ReadUInt16();
        Location targetLocation = (packet.ReadUInt16(), packet.ReadPoint());

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] CHKSUM: {nodeCheckSum} TARGET_LOCATION: {targetLocation}", client);
        Game.Game.ClickWorldMap(client, nodeCheckSum, targetLocation);
    }
    private void ClickObject(Client.Client client, ClientPacket packet)
    {
        var type = packet.ReadByte();
        switch (type)
        {
            case 1:
                var objectId = packet.ReadInt32();
                Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | OBJECT_ID: {objectId}", client);
                Game.Game.ClickObject(client, objectId);
                break;
            case 3:
                var clickPoint = packet.ReadPoint();
                Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | CLICK_POINT: {clickPoint}", client);
                Game.Game.ClickObject(client, clickPoint);
                break;
        }
    }
    private void RemoveEquipment(Client.Client client, ClientPacket packet)
    {
        var slot = (EquipmentSlot)packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SLOT: {slot}", client);
        Game.Game.RemoveEquipment(client, slot);
    }
    private void KeepAlive(Client.Client client, ClientPacket packet)
    {
        var b = packet.ReadByte();
        var a = packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.KeepAlive(client, a, b);
    }
    private void ChangeStat(Client.Client client, ClientPacket packet)
    {
        var stat = (Stat)packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] STAT: {stat}", client);
        Game.Game.ChangeStat(client, stat);
    }
    private void Exchange(Client.Client client, ClientPacket packet)
    {
        var type = (ExchangeRequestType)packet.ReadByte();
        var targetId = packet.ReadInt32();

        switch (type)
        {
            case ExchangeRequestType.StartExchange:
            {
                Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId}", client);
                Game.Game.Exchange(client, type, targetId);
                break;
            }
            case ExchangeRequestType.RequestAmount:
            {
                var slot = packet.ReadByte();

                Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId} | SOURCE_SLOT: {slot}", client);
                Game.Game.Exchange(client, type, targetId, 0, slot, 0);
                break;
            }
            case ExchangeRequestType.AddItem:
            {
                var slot = packet.ReadByte();
                var count = packet.ReadByte();

                Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId} | SOURCE_SLOT: {slot} | COUNT: {count}", client);
                Game.Game.Exchange(client, type, targetId, 0, slot, count);
                break;
            }
            case ExchangeRequestType.SetGold:
            {
                var amount = packet.ReadUInt32();

                Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId} | AMOUNT: {amount}", client);
                Game.Game.Exchange(client, type, targetId, amount);
                break;
            }
            case ExchangeRequestType.Cancel:
            case ExchangeRequestType.Accept:
                Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] TYPE: {type}", client);
                Game.Game.Exchange(client, type);
                break;
        }
    }
    private void RequestLoginNotification(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.RequestLoginNotification(true, client);
    }

    private void BeginChant(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.BeginChant(client);
    }
    private void DisplayChant(Client.Client client, ClientPacket packet)
    {
        var chant = packet.ReadString8();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] CHANT: {chant}", client);
        Game.Game.DisplayChant(client, chant);
    }
    private void Personal(Client.Client client, ClientPacket packet)
    {
        var totalLength = packet.ReadUInt16();
        var portraitLength = packet.ReadUInt16();
        var portraitData = packet.ReadBytes(portraitLength);
        var profileMsg = packet.ReadString16();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] DATA_SIZE: {portraitLength} | MESSAGE_LENGTH: {profileMsg.Length}", client);
        Game.Game.Personal(client, portraitData, profileMsg);
    }
    private void RequestServerTable(Client.Client client, ClientPacket packet)
    {
        var requestTable = packet.ReadBoolean();
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] REQUEST: {requestTable}", client);
        Game.Game.RequestServerTable(client, requestTable);
    }
    private void ChangeSequence(Client.Client client, ClientPacket packet) => client.ReceiveSequence = packet.Sequence;
    private void RequestHomepage(Client.Client client, ClientPacket packet)
    {
        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] ", client);
        Game.Game.RequestHomepage(client);
    }
    private void SynchronizeTicks(Client.Client client, ClientPacket packet)
    {
        var serverTicks = new TimeSpan(packet.ReadUInt32());
        var clientTicks = new TimeSpan(packet.ReadUInt32());

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] SERVER_TICKS: {serverTicks} | CLIENT_TICKS: {clientTicks}", client);
        Game.Game.SynchronizeTicks(client, serverTicks, clientTicks);
    }
    private void ChangeSocialStatus(Client.Client client, ClientPacket packet)
    {
        var status = (SocialStatus)packet.ReadByte();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] DESIRED_STATUS: {status}", client);
        Game.Game.ChangeSocialStatus(client, status);
    }
    private void RequestMetaFile(Client.Client client, ClientPacket packet)
    {
        var requestFile = packet.ReadBoolean();

        Server.WriteLogAsync($@"Recv [{(ClientOpCode)packet.OpCode}] REQUEST_FILE: {requestFile}", client);
        Game.Game.RequestMetaFile(client, requestFile);
    }
}*/

