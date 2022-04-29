/*
public class Client
{
    private readonly object Sync = new object();
    private readonly ImmutableArray<Handler> PacketHandlers;
    private readonly byte[] ClientBuffer;
    private readonly List<byte> FullClientBuffer;
    private bool Connected;

    public ConcurrentQueue<ServerPacket> SendQueue { get; }
    public Server.Server Server { get; }
    public IPAddress IpAddress { get; }

    public Socket ClientSocket { get; private set; }

    public bool IsLoopback { get; set; }
    public byte SendSequence { get; set; }
    public byte ReceiveSequence { get; set; }
    public byte StepCount { get; set; }
    public ServerType ServerType { get; set; }
    public Crypto Crypto { get; set; }
    public User User { get; set; }
    public string CreateCharName { get; set; }
    public string CreateCharPw { get; set; }
    public DateTime LastClickObj { get; set; }
    public DateTime LastRefresh { get; set; }
    public Dialog CurrentDialog { get; set; }
    public object ActiveObject { get; set; }


    /// <summary>
    /// Base constructor for a new client with reference to the server, and the user's socket.
    /// </summary>
    /// <param name="server">The game server.</param>
    /// <param name="socket">The client's socket.</param>
    public Client(Server.Server server, Socket socket)
    {
        Connected = false;
        ClientBuffer = new byte[4096];
        FullClientBuffer = new List<byte>();
        SendQueue = new ConcurrentQueue<ServerPacket>();

        Server = server;
        ClientSocket = socket;
        Crypto = new Crypto();
        PacketHandlers = ClientPackets.Handlers;
        SendSequence = 0;
        ReceiveSequence = 0;
        StepCount = 1;

        if (socket.RemoteEndPoint is IPEndPoint ipEndPoint)
            IpAddress = ipEndPoint.Address;

        LastClickObj = DateTime.MinValue;
        LastRefresh = DateTime.MinValue;
        IsLoopback = IpAddress.Equals(IPAddress.Loopback);
    }

    ~Client()
    {
        ClientSocket.Disconnect(false);
    }

    /// <summary>
    /// Adds the client to the client list, then begins receiving data.
    /// </summary>
    public void Connect()
    {
        Connected = true;

        if (Server.TryAddClient(this))
        {
            //when we receive data, copy the readable data to the client buffer and call endreceive
            ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), ClientSocket);

            if (ServerType != ServerType.World)
                Enqueue(ServerPackets.AcceptConnection());

            Old.Server.Server.WriteLogAsync("Connection accepted", this);
        }
        else
        {
            Connected = false;
            Old.Server.Server.WriteLogAsync("Connection failure", this);
        }
    }

    /// <summary>
    /// Disconnects the client from the server.
    /// </summary>
    /// <param name="wait">False if you want to immediately kill the client. True if you want the client to time out.</param>
    public void Disconnect()
    {
        lock (Sync)
        {
            try
            {
                if (Connected)
                {
                    Connected = false;

                    if (Server.TryRemoveClient(this))
                    {
                        if (User != null)
                            try { Game.World.RemoveClient(this); }
                            catch (Exception e) { Old.Server.Server.WriteLogAsync(e.Message, this); }
                        ClientSocket.Disconnect(false);
                    }

                    Old.Server.Server.WriteLogAsync("Connection terminated.", this);
                }
            }
            catch(Exception e)
            {
                Old.Server.Server.WriteLogAsync($"{Environment.NewLine}DISCONNECTION ERROR: {e.Message}{Environment.NewLine}");
            }
        }
    }

    /// <summary>
    /// Sends all packets in the send queue.
    /// </summary>
    public void FlushSendQueue()
    {
        if (!Connected)
            return;

        while (!SendQueue.IsEmpty)
        {
            if (SendQueue.TryDequeue(out var tServerPacket))
            {
                Old.Server.Server.WriteLogAsync(tServerPacket.LogString, this);

                if (tServerPacket.ShouldEncrypt)
                {
                    tServerPacket.Sequence = SendSequence++;
                    tServerPacket.Encrypt(Crypto);
                }
                Send(tServerPacket);
            }
        }
    }

    /// <summary>
    /// Asynchronously receives data from the client, and processes the information.
    /// </summary>
    /// <param name="ar">Result of the async operation.</param>
    private void ClientEndReceive(IAsyncResult ar)
    {
        try
        {
            //get the length of the packet
            var length = ClientSocket.EndReceive(ar);
            //if we receive a packet with no length, disconnect the client, something went wrong
            if (length == 0)
                Disconnect();
            else
            {
                //otherwise copy the client buffer into a new byte array sized to fit the length of the packet
                var data = new byte[length];
                Buffer.BlockCopy(ClientBuffer, 0, data, 0, length);
                //copy that array into the full client buffer, so we can deal with the information in a properly sized list
                FullClientBuffer.AddRange(data);
                while (FullClientBuffer.Count > 3)
                {
                    //check to see if it's a valid packet, this gives us the number of bytes that arent trailing random shit
                    var count = (FullClientBuffer[1] << 8) + FullClientBuffer[2] + 3;
                    if (count <= FullClientBuffer.Count)
                    {
                        //create a clientpacket out of the readable data
                        var clientPacket = new ClientPacket(FullClientBuffer.GetRange(0, count).ToArray());
                        //remove the data from the fullclientbuffer
                        FullClientBuffer.RemoveRange(0, count);

                        //send it off to be processed by the server
                        if (clientPacket != null)
                        {
                            //make sure the opcode is consistent with the client's current server type
                            if ((ServerType == ServerType.World)   ? CONSTANTS.WORLD_OPCODES.Contains(clientPacket.OpCode)
                                : (ServerType == ServerType.Login) ? CONSTANTS.LOGIN_OPCODES.Contains(clientPacket.OpCode)
                                : (ServerType == ServerType.Lobby) ? CONSTANTS.LOBBY_OPCODES.Contains(clientPacket.OpCode)
                                                                     : false)
                            {
                                if (clientPacket.ShouldEncrypt)
                                    clientPacket.Decrypt(Crypto);

                                if (clientPacket.IsDialog)
                                    clientPacket.DecryptDialog();

                                try
                                {
                                    PacketHandlers[clientPacket.OpCode](this, clientPacket);
                                }
                                catch (Exception e)
                                {
                                    Old.Server.Server.WriteLogAsync($@"{Environment.NewLine}HANDLER EXCEPTION: {e.Message}{Environment.NewLine}", this);
                                }
                            }
                            else
                                Old.Server.Server.WriteLogAsync($@"{Environment.NewLine}INVALID PACKET[{clientPacket.OpCode}]: {ServerType} => {clientPacket}{Environment.NewLine}");
                        }
                    }
                    else
                        break;
                }
            }
        }
        catch(Exception e)
        {
            Old.Server.Server.WriteLogAsync($@"{Environment.NewLine}ENDRECEIVE EXCEPTION: {e.ToString()}{Environment.NewLine}");
        }
        finally
        {
            if (Connected) //begin checking for received info again
                ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), ClientSocket);
        }
    }

    /// <summary>
    /// Asynchronously finalizes the sending of a packet.
    /// </summary>
    /// <param name="ar">Result of the async operation.</param>
    private void EndSend(IAsyncResult ar) => ((Socket)ar.AsyncState).EndSend(ar);

    /// <summary>
    /// Queues multiple packets to be sent to the client.
    /// </summary>
    /// <param name="packets">Packet(s) to be sent.</param>
    public void Enqueue(params ServerPacket[] packets)
    {
        for (var i = 0; i < packets.Length; i++)
            if (packets[i] != null)
                SendQueue.Enqueue(packets[i]);
    }

    /// <summary>
    /// Begins an asynchronous operation to send a packet to the client.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    private void Send(ServerPacket packet)
    {
        var data = packet.ToArray();
        try { ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), ClientSocket); }
        catch
        {
            //do things to save the connection?
        }
    }

    /// <summary>
    /// Redirects the client to another server, or in this case... the same server.
    /// </summary>
    /// <param name="redirect">Redirect information.</param>
    public void Redirect(Redirect redirect)
    {
        Server.Redirects.Add(redirect);
        Enqueue(ServerPackets.Redirect(redirect));
    }

    /// <summary>
    /// Sends a message to the client when theyre at the login screen.
    /// </summary>
    public void SendLoginMessage(LoginMessageType messageType, string message = "") => Enqueue(ServerPackets.LoginMessage(messageType, message));

    /// <summary>
    /// Refreshes the clients view of their current hp, mp and other attributes.
    /// </summary>
    public void SendAttributes(StatUpdateType updateType) => Enqueue(ServerPackets.Attributes(User.IsAdmin, updateType, User.Attributes));

    /// <summary>
    /// Sends a message to the client when they're already logged in.
    /// </summary>
    public void SendServerMessage(ServerMessageType messageType, string message) => Enqueue(ServerPackets.ServerMessage(messageType, message));

    /// <summary>
    /// Sends a public message and it's origins to the client.
    /// </summary>
    /// <param name="sourceId">ID of the source of the message.</param>
    public void SendPublicMessage(PublicMessageType messageType, int sourceId, string message) => Enqueue(ServerPackets.PublicChat(messageType, sourceId, message));

    /// <summary>
    /// Sends an animation to the client and adds it to the animation history.
    /// </summary>
    /// <param name="animation">The animation to send.</param>
    public void SendAnimation(Animation animation) => Enqueue(ServerPackets.Animation(animation));

    /// <summary>
    /// Sends an effect to the client's spellbar.
    /// </summary>
    /// <param name="eff">The effect to send.</param>
    /// <param name="remove">Whether or not to remove the effect from the spell bar.</param>
    public void SendEffect(Effect eff)
    {
        if(eff.ShouldSendColor())
            Enqueue(ServerPackets.EffectsBar(eff.Sprite, eff.Color));
    }
    /// <summary>
    /// Sends a persuit menu to the client. Sets necessary client variables.
    /// </summary>
    /// <param name="merchant">Merchant with a merchantmenu.</param>
    public void SendMenu(Merchant merchant)
    {
        ActiveObject = merchant;
        if (merchant.Menu.Type == MenuType.Dialog)
        {
            CurrentDialog = (merchant.NextDialogId == 0) ? Game.Dialogs.ItemOrMerchantMenuDialog(PursuitId.None, 0, merchant.Menu.Text, merchant.Menu.Dialogs) : Game.Dialogs[merchant.NextDialogId];

            Enqueue(ServerPackets.DisplayMenu(this, merchant, CurrentDialog));
        }
        else
            Enqueue(ServerPackets.DisplayMenu(this, merchant));
    }

    /// <summary>
    /// Sends a dialog to the client. Sets necessary client variables.
    /// </summary>
    /// <param name="invoker">Item or Merchant that's the source of the dialog.</param>
    /// <param name="dialog">The dialog to be sent.</param>
    public void SendDialog(object invoker, Dialog dialog)
    {
        if (dialog.Type != DialogType.CloseDialog)
        {
            ActiveObject = invoker;
            CurrentDialog = dialog;
        }
        else
        {
            ActiveObject = null;
            CurrentDialog = null;
        }

        Enqueue(ServerPackets.DisplayDialog(invoker, dialog));
    }

    /// <summary>
    /// Refreshes the client, resending all on-screen information.
    /// </summary>
    public void Refresh(bool byPassTimer = false) => User.Map.Refresh(this, byPassTimer);
}*/

