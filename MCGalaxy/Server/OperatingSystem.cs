/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MCGalaxy 
{
    public struct CPUTime
    {
        public ulong IdleTime, KernelTime, UserTime;
    }

    public abstract class IOperatingSystem
    {
        /// <summary> Measures CPU use by all processes in the system </summary>
        public abstract CPUTime MeasureAllCPUTime();
        /// <summary> Attempts to restart the server process in-place </summary>
        /// <remarks> Does not return when restart is successful 
        /// (since current process image is replaced) </remarks>
        public abstract void RestartProcess();


        public unsafe static IOperatingSystem DetectOS() {
            PlatformID platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Win32NT || platform == PlatformID.Win32Windows)
                return new WindowsOS();

            sbyte* ascii  = stackalloc sbyte[8192];
            uname(ascii);
            string kernel = new String(ascii);

            if (kernel == "Darwin") return new macOS();

            return new UnixOS();
        }

        [DllImport("libc")]
        unsafe static extern void uname(sbyte* uname_struct);
    }

    class WindowsOS : IOperatingSystem
    {
        public override CPUTime MeasureAllCPUTime() {
            CPUTime all = default(CPUTime);
            GetSystemTimes(out all.IdleTime, out all.KernelTime, out all.UserTime);

            // https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getsystemtimes
            // lpKernelTime - "... This time value also includes the amount of time the system has been idle."
            all.KernelTime -= all.IdleTime;
            return all;
        }

        [DllImport("kernel32.dll")]
        static extern int GetSystemTimes(out ulong idleTime, out ulong kernelTime, out ulong userTime);

        public override void RestartProcess() { }
    }

    class macOS : UnixOS
    {
        // https://stackoverflow.com/questions/20471920/how-to-get-total-cpu-idle-time-in-objective-c-c-on-os-x
        // /usr/include/mach/host_info.h, /usr/include/mach/machine.h, /usr/include/mach/mach_host.h
        public override CPUTime MeasureAllCPUTime() {
            uint[] info = new uint[4]; // CPU_STATE_MAX
            uint count  = 4; // HOST_CPU_LOAD_INFO_COUNT 
            int flavor  = 3; // HOST_CPU_LOAD_INFO
            host_statistics(mach_host_self(), flavor, info, ref count);

            CPUTime all;
            all.IdleTime   = info[2]; // CPU_STATE_IDLE
            all.UserTime   = info[0] + info[3]; // CPU_STATE_USER + CPU_STATE_NICE
            all.KernelTime = info[1]; // CPU_STATE_SYSTEM
            return all;
        }

        [DllImport("libc")]
        static extern IntPtr mach_host_self();
        [DllImport("libc")]
        static extern int host_statistics(IntPtr port, int flavor, uint[] info, ref uint count);
    }

    class UnixOS : IOperatingSystem
    {
        // https://stackoverflow.com/questions/15145241/is-there-an-equivalent-to-the-windows-getsystemtimes-function-in-linux
        public override CPUTime MeasureAllCPUTime() {
            try {
                using (StreamReader r = new StreamReader("/proc/stat"))
                {
                    string line = r.ReadLine();
                    if (line.StartsWith("cpu ")) return ParseCpuLine(line);
                }
            } catch (FileNotFoundException) { }

            return default(CPUTime);
        }

        static CPUTime ParseCpuLine(string line) {
            // Linux : cpu  [USER TIME] [NICE TIME] [SYSTEM TIME] [IDLE TIME] [I/O WAIT TIME] [IRQ TIME] [SW IRQ TIME]
            // NetBSD: cpu [USER TIME] [NICE TIME] [SYSTEM TIME] [IDLE TIME]
            line = line.Replace("  ", " ");
            string[] bits = line.SplitSpaces();

            ulong user = ulong.Parse(bits[1]);
            ulong nice = ulong.Parse(bits[2]);
            ulong kern = ulong.Parse(bits[3]);
            ulong idle = ulong.Parse(bits[4]);
            // TODO interrupt time too?

            CPUTime all;
            all.UserTime   = user + nice;
            all.KernelTime = kern;
            all.IdleTime   = idle;
            return all;
        }


        public override void RestartProcess() {
            if (Server.CLIMode) HACK_Execvp();
        }

#if !NETSTANDARD
        [DllImport("libc", SetLastError = true)]
        static extern int execvp(string path, string[] argv);
        
        static void HACK_Execvp() {
            // With using normal Process.Start with mono, after Environment.Exit
            //  is called, all FDs (including standard input) are also closed.
            // Unfortunately, this causes the new server process to constantly error with
            //   Type: IOException
            //   Message: Invalid handle to path "server_folder_path/[Unknown]"
            //   Trace:   at System.IO.FileStream.ReadData (System.Runtime.InteropServices.SafeHandle safeHandle, System.Byte[] buf, System.Int32 offset, System.Int32 count) [0x0002d]
            //     at System.IO.FileStream.ReadInternal (System.Byte[] dest, System.Int32 offset, System.Int32 count) [0x00026]
            //     at System.IO.FileStream.Read (System.Byte[] array, System.Int32 offset, System.Int32 count) [0x000a1] 
            //     at System.IO.StreamReader.ReadBuffer () [0x000b3]
            //     at System.IO.StreamReader.Read () [0x00028]
            //     at System.TermInfoDriver.GetCursorPosition () [0x0000d]
            //     at System.TermInfoDriver.ReadUntilConditionInternal (System.Boolean haltOnNewLine) [0x0000e]
            //     at System.TermInfoDriver.ReadLine () [0x00000]
            //     at System.ConsoleDriver.ReadLine () [0x00000]
            //     at System.Console.ReadLine () [0x00013]
            //     at MCGalaxy.Cli.CLI.ConsoleLoop () [0x00002]
            // (this errors multiple times a second and can quickly fill up tons of disk space)
            // And also causes console to be spammed with '1R3;1R3;1R3;' or '363;1R;363;1R;'
            //
            // Note this issue does NOT happen with GUI mode for some reason - and also
            // don't want to use excevp in GUI mode, otherwise the X socket FDs pile up

            // try to exec using actual runtime path first, e.g. /usr/bin/mono-sgen
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            execvp(exe, new string[] { exe, Server.GetRestartPath() });
            Console.WriteLine("execvp {0} failed: {1}", exe, Marshal.GetLastWin32Error());

            // .. and fallback to mono if that doesn't work for some reason
            execvp("mono", new string[] { "mono", Server.GetRestartPath() });
            Console.WriteLine("execvp mono failed: {0}", Marshal.GetLastWin32Error());
        }
#else
        [DllImport("libc", SetLastError = true)]
        unsafe static extern int execvp(byte* path, byte** argv);
        
        unsafe static void HACK_Execvp() {
            // similar issue as with Mono, but happens with this instead
            //  "IOException with 'I/O error' message
            //     ...
            //     at System.IO.StdInReader.ReadKey()
            //
            // Trying to use heap allocated string sometimes causes EFAULT error,
            //  therefore manually allocate arguments on the stack instead
            byte* path  = stackalloc byte[8192];
            byte** args = stackalloc byte*[2];
            args[0] = path;
            args[1] = (byte*)IntPtr.Zero;

            byte[] tmp = Encoding.UTF8.GetBytes(Server.GetRestartPath());
            Marshal.Copy(tmp, 0, (IntPtr)path, tmp.Length);

            execvp(path, args);
            Console.WriteLine("execvp failed: {0}", Marshal.GetLastWin32Error());
        }
#endif
    }
}