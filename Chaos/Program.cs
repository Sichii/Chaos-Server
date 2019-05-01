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

namespace Chaos
{
    public class Program
    {
        private static Server Server;
        private static void Main(string[] args)
        {
            Console.Title = "Chaos Server";
            Console.WindowWidth = 200;
            Console.WindowHeight = 30;

            Paths.Set();

            //create the server, start it in a new thread
            IPAddress localIP =
#if DEBUG
            IPAddress.Loopback;
#else
            Dns.GetHostEntry(Paths.HostName).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
#endif
            Server = new Server(localIP);

            //this thread will block for command line input for use as an admin panel
            while (Server.Running)
                Console.ReadLine(); //we can do server commands here when the time comes
        }
    }
}
