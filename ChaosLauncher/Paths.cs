using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosLauncher
{
    internal static class Paths
    {
        internal static string BaseDir = "";
        //dark ages directory, populated by setpaths
        internal static string DarkAgesDir => $@"{BaseDir}ChaosDa\";
        //dark ages executable
        internal static string DarkAgesExe => $@"{DarkAgesDir}Darkages.exe";
        //dynamic host name, populated by setpaths
        internal static string HostName = "";
    }
}
