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

namespace Chaos
{
    static class Paths
    {
        //primary directory, change to your own~
        internal static string Dir => @"C:\Users\Sichi\Desktop\ChaosProject\";
        //dynamic host name, change to your own~
        internal static string HostName => @"ChaosServer.dynu.net";
        //redis config string (host, port)
        internal static string RedisConfig => @"localhost:6379";


        internal static string LogFiles => $@"{Dir}logs\";
        internal static string MetaFiles => $@"{Dir}metafiles\";
        internal static string MapFiles => $@"{Dir}maps\";


    }
}
