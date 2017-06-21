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
            Console.SetCursorPosition(0, Console.WindowHeight - 1);

            //display dns ip for others to connect to
            Console.WriteLine($"Server IP: {Dns.GetHostAddresses("accoserver.dynu.com")[0]}");

            if (!File.Exists(Paths.UserHash))
                File.Create(Paths.UserHash);

            if (!Directory.Exists(Paths.Chars))
                Directory.CreateDirectory(Paths.Chars);

            //create the server, start it in a new thread
            Server = new Server(IPAddress.Any, 2610);
            ServerThread = new Thread(Server.Start);
            ServerThread.Start();

            //this thread will block for command line input for use as an admin panel
            while (Server.ServerSocket.Connected)
                Console.ReadLine(); //we can do server commands here when the time comes
        }
    }
}
