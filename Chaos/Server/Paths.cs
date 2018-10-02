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

namespace Chaos
{
    /// <summary>
    /// Contains physical paths for the chaos-server project folder, and the server's local hostname.
    /// </summary>
    public static class Paths
    {
        //primary directory, populated by setpaths
        private static string BaseDir;

        //redis config string (host, port)
        internal const string RedisConfig = @"localhost:6379";
        //dynamic host name, populated by setpaths
        internal static string HostName = "";

        internal static string LogFiles => $@"{BaseDir}logs\";
        internal static string MetaFiles => $@"{BaseDir}metafiles\";
        internal static string MapFiles => $@"{BaseDir}maps\";

        public static void Set()
        {
            BaseDir = Properties.Resources.PATH[0].Trim('\n', '\r');
            HostName = Properties.Resources.PATH[1].Trim('\n', '\r', ' ');

            if (!BaseDir.EndsWith(@"\"))
                BaseDir += @"\";
        }
    }
}
