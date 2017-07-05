using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Chaos
{
    internal sealed class Client
    {
        private bool Connected = false;
        private byte ServerType { get; set; } //0 = lobby, 1 = login, 2 = world
        private byte[] ClientBuffer = new byte[4096];
        private List<byte> FullClientBuffer = new List<byte>();
        private Queue<ServerPacket> SendQueue = new Queue<ServerPacket>();
        private Queue<ClientPacket> ProcessQueue = new Queue<ClientPacket>();
        private byte ServerSequence = 0;
        internal Server Server { get; }
        internal Socket ClientSocket { get; }
        internal Crypto Crypto { get; set; }
        internal Objects.User User { get; set; }

        internal string NewCharName;
        internal string NewCharPw;

        /// <summary>
        /// Creates a new user with reference to the server, and the user's socket.
        /// </summary>
        /// <param name="server">The game server.</param>
        /// <param name="socket">The user's socket.</param>
        internal Client(Server server, Socket socket)
        {
            Server = server;
            ServerType = 0;
            ClientSocket = socket;
            Crypto = new Crypto();

        }

        /// <summary>
        /// Connects to the socket and begins receiving data.
        /// </summary>
        internal void Connect()
        {
            ClientSocket.Connect(ClientSocket.RemoteEndPoint);
            Connected = true;
            Enqueue(ServerPackets.AcceptConnection());

            //when we receive data, copy the readable data to the client buffer and call endreceive
            ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), null);
        }

        /// <summary>
        /// Disconnects the user from the server.
        /// </summary>
        /// <param name="wait">False if you want to immediately kill the client. True if you want the client to time out.</param>
        internal void Disconnect(bool wait = false)
        {
            Connected = false;
            if (wait)
                return;

            Client dis = this;
            if (Server.World.Clients.TryRemove(ClientSocket, out dis))
                ClientSocket.Disconnect(false);
        }

        /// <summary>
        /// Asynchronous operation to receive information from the client
        /// </summary>
        /// <param name="ar">Result of the async operation.</param>
        private void ClientEndReceive(IAsyncResult ar)
        {
            //get the length of the packet
            int length = ClientSocket.EndReceive(ar);
            //if we receive a packet with no length, disconnect the client, something went wrong
            if (length == 0)
                Disconnect();
            else
            {
                //otherwise copy the client buffer into a new byte array sized to fit the length of the packet
                byte[] numArray = new byte[length];
                Array.Copy(ClientBuffer, numArray, length);
                //copy that array into the full client buffer, so we can deal with the information in a properly sized list
                FullClientBuffer.AddRange(numArray);
                while (FullClientBuffer.Count > 3)
                {
                    //check to see if it's a valid packet, this gives us the number of bytes that arent trailing random shit
                    int count = (FullClientBuffer[1] << 8) + FullClientBuffer[2] + 3;
                    if (count <= FullClientBuffer.Count)
                    {
                        //copy the non-random shit into a new list, remove it from the client buffer
                        List<byte> range = FullClientBuffer.GetRange(0, count);
                        FullClientBuffer.RemoveRange(0, count);
                        //create a clientpacket out of the readable data, and send it to be processed by the server
                        ClientPacket clientPacket = new ClientPacket(range.ToArray());
                        lock (ProcessQueue)
                            ProcessQueue.Enqueue(clientPacket);
                    }
                    else
                        break;
                }
                //begin checking for received info again
                ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), null);
            }
        }

        /// <summary>
        /// Asynchronous operation to end a pending send.
        /// </summary>
        /// <param name="ar">Result of the async operation.</param>
        private void EndSend(IAsyncResult ar) => ((Socket)ar.AsyncState).EndSend(ar);

        /// <summary>
        /// Sends packets to the process/send thread.
        /// </summary>
        /// <param name="packets">Packet(s) to be sent.</param>
        internal void Enqueue(params ServerPacket[] packets)
        {
            lock (SendQueue)
            {
                for (int i = 0; i < packets.Length; i++)
                    SendQueue.Enqueue(packets[i]);
            }
        }

        /// <summary>
        /// Thread/Loop where packets from the client are processed, and the server sends packets back to the client.
        /// </summary>
        private void ClientLoop()
        {
            while (Connected)
            {
                //we send server packets
                lock (SendQueue)
                {
                    //while there are packets to send
                    while (SendQueue.Count > 0)
                    {
                        //get the next packet in the queue, convert to serverpacket
                        ServerPacket packet = SendQueue.Dequeue();
                        if (packet == null) continue;

                        //if it should be encrypted, do it
                        if (packet.ShouldEncrypt)
                        {
                            packet.Sequence = ServerSequence++;
                            packet.Encrypt(Crypto);
                        }
                        //get the packet's data and try to send it
                        byte[] data = packet.ToArray();
                        try { ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), ClientSocket); }
                        catch { }
                    }
                }
                //and receive client packets
                lock (ProcessQueue)
                {
                    //while there are packets to process
                    while (ProcessQueue.Count > 0)
                    {
                        //get the next packet in the queue, convert to clientpacket
                        ClientPacket packet = ProcessQueue.Dequeue();
                        if (packet == null) continue;

                        //if it is encrypted, decrypt it
                        if (packet.ShouldEncrypt)
                            packet.Decrypt(Crypto);
                        //if packet is a dialog, decrypt the header
                        if (packet.IsDialog)
                            packet.DecryptDialog();

                        //get the handler for this packet
                        ClientPacketHandler handle = Server.ClientPacketHandlers[packet.OpCode];
                        //if we have a handler for this packet
                        if (handle != null)
                            //no srsly, please handle it
                            try { handle(this, packet); }
                            catch { }
                    }
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Changes updates the location of the client in the path to logging in.
        /// </summary>
        /// <param name="redirect">Redirect information.</param>
        internal void Redirect(Redirect redirect)
        {
            ServerType = redirect.Type;
        }
    }
}
