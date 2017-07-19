using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Chaos
{
    internal class Program
    {
        private static Server Server;
        private static Thread ServerThread;
        private static void Main(string[] args)
        {
            Console.Title = "Chaos Server";

            //create the server, start it in a new thread
            Server = new Server(IPAddress.Any, 2610);
            Server.WriteLog($"Loading world, please wait...");
            ServerThread = new Thread(Server.Start);
            ServerThread.Start();

            //display dns ip for others to connect to
            Server.WriteLog($"Server IP: {Dns.GetHostAddresses("accoserver.dynu.com")[0]}");

            while (Server.ServerSocket == null)
                Thread.Sleep(10);

            Server.WriteLog($"Server is connected and listening.");

            //this thread will block for command line input for use as an admin panel
            while (Server.ServerSocket != null)
                Console.ReadLine(); //we can do server commands here when the time comes
        }
    }
}
