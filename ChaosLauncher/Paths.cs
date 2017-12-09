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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosLauncher
{
    internal static class Paths
    {
        internal static string BaseDir => @"C:\Users\Sichi\Desktop\ChaosProject\";
        internal static string HostName => "chaosserver.dynu.net";

        internal static string DarkAgesDir => $@"{BaseDir}ChaosDa\";
        internal static string DarkAgesExe => $@"{DarkAgesDir}Darkages.exe";
    }
}
