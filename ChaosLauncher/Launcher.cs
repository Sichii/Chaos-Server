using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChaosLauncher
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void launchBtn_Click(object sender, EventArgs e)
        {
            StartInfo startInfo = new StartInfo();
            ProcInfo procInfo = new ProcInfo();
            startInfo.Size = Marshal.SizeOf(startInfo);

            //create the process
            SafeNativeMethods.CreateProcess(
#if DEBUG
                Paths.DarkAgesExe
#else
                @"Darkages.exe"
#endif
                , null, IntPtr.Zero, IntPtr.Zero, false, ProcessCreationFlags.Suspended, IntPtr.Zero, null, ref startInfo, out procInfo);

            //grab the process we created
            Process proc = Process.GetProcessById(procInfo.ProcessId);


            if (injectDawndCbox.Checked)
            {
                //get a handle for access
                IntPtr accessHnd = SafeNativeMethods.OpenProcess(ProcessAccess.All, true, (uint)proc.Id);
                //use access handle to inject dawnd.dll
                InjectDLL(accessHnd,
#if DEBUG
                    $@"{Paths.DarkAgesDir}dawnd.dll"
#else
                    "dawnd.dll"
#endif
                    );
            }

            using (ProcMemoryStream memory = new ProcMemoryStream(procInfo, ProcessAccess.VmOperation | ProcessAccess.VmRead | ProcessAccess.VmWrite))
            {
                //force "socket" - call for direct ip
                memory.Position = 0x4333A2;
                memory.WriteByte(0xEB);

                //change direct ip
                byte[] address = Dns.GetHostEntry(Paths.HostName).AddressList.FirstOrDefault(ip => ip.GetAddressBytes().Length == 4).GetAddressBytes();
                memory.Position = 0x4333C2;
                memory.WriteByte(106);
                memory.WriteByte(address[3]);
                memory.WriteByte(106);
                memory.WriteByte(address[2]);
                memory.WriteByte(106);
                memory.WriteByte(address[1]);
                memory.WriteByte(106);
                memory.WriteByte(address[0]);

                //skip intro
                memory.Position = 0x42E61F;
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);
                memory.WriteByte(0x90);

                //allow multiple instances
                memory.Position = 0x57A7D9;
                memory.WriteByte(0xEB);

                //resume process
                memory.Position = 0x6F3CA4;
                SafeNativeMethods.ResumeThread(procInfo.ThreadHandle);
            }

            //let process render it's window before we change the title
            while (proc.MainWindowHandle == IntPtr.Zero) { }
            //set window title
            SafeNativeMethods.SetWindowText(proc.MainWindowHandle, "Chaos");
        }

        public void InjectDLL(IntPtr processHandle, string dllName)
        {
            IntPtr outStuff;

            //length of string containing the DLL file name +1 byte padding
            int dllNameLength = dllName.Length + 1;
            //allocate memory within the virtual address space of the target process
            IntPtr processMemoryBase = SafeNativeMethods.VirtualAllocEx(processHandle, (IntPtr)null, (UIntPtr)dllNameLength, 0x1000, 0x40);
            //write DLL file name to allocated memory in target process
            SafeNativeMethods.WriteProcessMemory(processHandle, processMemoryBase, dllName, (UIntPtr)dllNameLength, out outStuff);
            //function pointer "Injector"
            UIntPtr moduleAddress = SafeNativeMethods.GetProcAddress(SafeNativeMethods.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (moduleAddress == null)
            {
                //invalid address
                MessageBox.Show(this, "Invalid module address.");
                return;
            }

            //create thread in target process, and store handle
            IntPtr threadHandle = SafeNativeMethods.CreateRemoteThread(processHandle, (IntPtr)null, UIntPtr.Zero, moduleAddress, processMemoryBase, 0, out outStuff);
            //make sure thread handle is valid
            if (threadHandle == null)
            {
                //invalid thread handle
                MessageBox.Show(this, "Invalid thread handle.");
                return;
            }
            //time-out is 5 seconds...
            WaitEventResult result = (WaitEventResult)SafeNativeMethods.WaitForSingleObject(threadHandle, 5000);
            //check whether thread timed out...
            if (result != WaitEventResult.Signaled)
            {
                //thread timed out...
                MessageBox.Show(this, "Thread timed out.");
                //make sure thread handle is valid before closing... prevents crashes.
                if (threadHandle != null)
                    SafeNativeMethods.CloseHandle(threadHandle);
                return;
            }
            //clear up allocated space ( Allocmem )
            SafeNativeMethods.VirtualFreeEx(processHandle, processMemoryBase, UIntPtr.Zero, 0x8000);
            //make sure thread handle is valid before closing... prevents crashes.
            if (threadHandle != null)
                SafeNativeMethods.CloseHandle(threadHandle);
            //return succeeded
            return;
        }
    }
}
