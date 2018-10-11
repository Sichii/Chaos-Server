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

namespace ChaosLauncher
{
    internal static class Paths
    {
        private static string BaseDir = "";
        //dark ages directory, populated by setpaths

#if DEBUG
        internal static string DarkAgesDir => $@"{BaseDir}ChaosDa\";
        //dark ages executable
        internal static string DarkAgesExe => $@"{DarkAgesDir}Darkages.exe";
#else
        internal static string DarkAgesExe => "Darkages.exe";
#endif
        //dynamic host name, populated by setpaths
        internal static string HostName = "";

        public static void Set()
        {
            string[] paths = Properties.Resources.PATH.Split('\n');

            BaseDir = paths[0].Trim('\n', '\r');
            HostName = paths[1].Trim('\n', '\r', ' ');

            if (!BaseDir.EndsWith(@"\"))
                BaseDir += @"\";
        }
    }
}
