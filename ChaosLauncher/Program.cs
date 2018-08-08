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
using System.Windows.Forms;

namespace ChaosLauncher
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            SetPaths();

            Application.Run(new Launcher());
        }

        public static void SetPaths()
        {
            Paths.BaseDir = Properties.Resources.PATH[0].Trim('\n', '\r');
            Paths.HostName = Properties.Resources.PATH[1].Trim('\n', '\r', ' ');

            if (!Paths.BaseDir.EndsWith(@"\"))
                Paths.BaseDir += @"\";
        }
    }
}
