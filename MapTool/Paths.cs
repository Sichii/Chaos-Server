namespace ChaosTool
{
    internal static class Paths
    {
        private static string BaseDir = "";
        //dark ages directory, populated by setpaths
        internal static string DarkAgesDir => $@"{BaseDir}ChaosDa\";
        //dark ages executable
        internal static string DarkAgesExe => $@"{DarkAgesDir}Darkages.exe";
        //dynamic host name, populated by setpaths

        public static void Set()
        {
            var paths = Properties.Resources.PATH.Split('\n');

            BaseDir = paths[0].Trim('\n', '\r');

            if (!BaseDir.EndsWith(@"\"))
                BaseDir += @"\";
        }
    }
}
