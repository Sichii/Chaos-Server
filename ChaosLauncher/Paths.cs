using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosLauncher
{
    internal static class Paths
    {
        internal static string BaseDir => @"C:\Users\mewbb\Desktop\chaosmap\";
        internal static string HostName => "localhost";

        internal static string DarkAgesDir => $@"{BaseDir}Dark Ages\";
        internal static string DarkAgesExe => $@"{DarkAgesDir}Darkages.exe";
    }
}
