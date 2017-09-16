using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChaosLauncher
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowText(IntPtr hWnd, string text);
    }
}
