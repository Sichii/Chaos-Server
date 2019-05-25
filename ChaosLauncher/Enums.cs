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

namespace ChaosLauncher
{
    [Flags]
    internal enum ProcessAccess
    {
        None = 0,
        Terminate = 0x0001,
        CreateThread = 0x0002,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        DuplicateHandle = 0x0040,
        CreateProcess = 0x0080,
        SetQuota = 0x0100,
        SetInformation = 0x0200,
        QueryInformation = 0x0400,
        SuspendResume = 0x0800,
        QueryLimitedInformation = 0x1000,
        Synchronize = 0x00100000,
        All = 0x1F0FFF
    }
    [Flags]
    internal enum ProcessCreationFlags
    {
        DebugProcess = 0x0001,
        DebugOnlyThisProcess = 0x0002,
        Suspended = 0x0004,
        DetachedProcess = 0x0008,
        NewConsole = 0x0010,
        NewProcessGroup = 0x0200,
        UnicodeEnvironment = 0x0400,
        SeparateWowVdm = 0x0800,
        SharedWowVdm = 0x1000,
        InheritParentAffinity = 0x00010000,
        ProtectedProcess = 0x00040000,
        ExtendedStartupInfoPresent = 0x00080000,
        BreakawayFromJob = 0x01000000,
        PreserveCodeAuthZLevel = 0x02000000,
        DefaultErrorMode = 0x04000000,
        NoWindow = 0x08000000,
    }
}
