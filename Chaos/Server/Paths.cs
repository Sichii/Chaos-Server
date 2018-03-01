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
    public static class Paths
    {
        //primary directory, change to your own~
        public static string BaseDir => @"C:\Users\Sichi\Desktop\ChaosProject\";
        //dark ages directory
        public static string DarkAgesDir => $@"{BaseDir}ChaosDa\";
        //dark ages executable
        public static string DarkAgesExe => $@"{DarkAgesDir}Darkages.exe";
        //dynamic host name, change to your own~
        public static string HostName => @"ChaosServer.dynu.net";
        //redis config string (host, port)
        public static string RedisConfig => @"localhost:6379";


        public static string LogFiles => $@"{BaseDir}logs\";
        public static string MetaFiles => $@"{BaseDir}metafiles\";
        public static string MapFiles => $@"{BaseDir}maps\";


    }
}
