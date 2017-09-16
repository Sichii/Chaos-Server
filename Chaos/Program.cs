using System;
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
            Console.WindowWidth = 150;
            Console.WindowHeight = 30;

            //create the server, start it in a new thread
            Server = new Server(IPAddress.Any, 2610);
            ServerThread = new Thread(Server.Start);
            ServerThread.Start();

            while (Server.ServerSocket == null)
                Thread.Sleep(10);

            Server.WriteLog("Server is ready");

            //this thread will block for command line input for use as an admin panel
            while (Server.ServerSocket != null)
                Console.ReadLine(); //we can do server commands here when the time comes
        }
    }
}
