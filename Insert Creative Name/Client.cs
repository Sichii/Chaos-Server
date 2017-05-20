using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal class Client
    {
        private Server Server;
        internal Socket ClientSocket;
        private bool Connected;
        private byte[] ClientBuffer;
        private List<byte> FullClientBuffer;
        private Queue<Packet> SendQueue;
        private Queue<Packet> ProcessQueue;
        private byte ClientSequence;
        private byte ServerSequence;
        internal Crypto Crypto;
        private Attributes Stats;

        //creates a new user with reference to the server, and the user's socket
        internal Client(Server server, Socket socket)
        {
            Server = server;
            ClientSocket = socket;
            ClientBuffer = new byte[4096];
            FullClientBuffer = new List<byte>();
            SendQueue = new Queue<Packet>();
            ProcessQueue = new Queue<Packet>();
            Crypto = new Crypto(0, "UrkcnItnI");
            Stats = new Attributes();
        }

        //connects to the socket and begins receiving data
        internal void Connect()
        {
            ClientSocket.Connect(ClientSocket.RemoteEndPoint);
            Connected = true;

            //when we receive data, copy the readable data to the client buffer and call endreceive
            ClientSocket.BeginReceive(ClientBuffer, 0, ClientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), null);
        }

        //disconnects the user from the server
        internal void Disconnect(bool wait = false)
        {
            Connected = false;
            if (wait)
                return;

            Client dis = this;
            if(Server.Clients.TryRemove(ClientSocket, out dis))
                ClientSocket.Disconnect(false);
        }

        //this is how the server gets information from the client
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

        private void EndSend(IAsyncResult ar)
        {
            ((Socket)ar.AsyncState).EndSend(ar);
        }

        //sends packets to the process/send thread
        internal void Enqueue(params Packet[] packets)
        {
            lock (SendQueue)
            {
                Packet[] packets2 = packets;
                for (int i = 0; i < packets2.Length; i++)
                {
                    Packet item = packets2[i];
                    SendQueue.Enqueue(item);
                }
            }
        }

        //this is where we process information the client sends us, or send the client information
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
                        ServerPacket packet = SendQueue.Dequeue() as ServerPacket;
                        if (packet == null) continue;

                        //if it should be encrypted, do it
                        if (packet.ShouldEncrypt)
                        {
                            packet.Sequence = ServerSequence++;
                            packet.Encrypt(Crypto);
                        }
                        //get the packet's data and try to send it
                        byte[] data = packet.ToArray();
                        try
                        {
                            ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), ClientSocket);
                        }
                        catch { }
                    }
                }
                //and receive client packets
                lock (ProcessQueue)
                {
                    //while there are packets to process
                    while (ProcessQueue.Count > 0)
                    {
                        //get the next packet in the queue, conver to clientpacket
                        ClientPacket packet = ProcessQueue.Dequeue() as ClientPacket;
                        if (packet == null) continue;

                        //if it is encrypted, decrypt it
                        if (packet.ShouldEncrypt)
                            packet.Decrypt(Crypto);
                        //if packet is a dialog, decrypt it
                        if (packet.IsDialog)
                            packet.DecryptDialog();

                        //get the handler for this packet
                        ClientPacketHandler handle = Server.ClientPacketHandlers[packet.OpCode];
                        //if we have a handler for this packet
                        if (handle != null)
                            //lock the server for synchronization
                            lock (Server.SyncObj)
                            {
                                //process the packet
                                try { handle(this, packet); }
                                catch { }
                            }
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}
