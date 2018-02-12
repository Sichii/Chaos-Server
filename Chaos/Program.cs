// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
            IPAddress localIP = Dns.GetHostEntry(Paths.HostName).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            Server = new Server(localIP, 2610);
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
