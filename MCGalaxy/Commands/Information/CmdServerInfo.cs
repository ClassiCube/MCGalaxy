/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Threading;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Info 
{
    public sealed class CmdServerInfo : Command2 
    {
        public override string name { get { return "ServerInfo"; } }
        public override string shortcut { get { return "SInfo"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Host"), new CommandAlias("ZAll") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "can see server CPU and memory usage") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            int count = Database.CountRows("Players");
            p.Message("About &b{0}&S", Server.Config.Name);
            p.Message("  &a{0} &Splayers total. (&a{1} &Sonline, &8{2} banned&S)",
                      count, PlayerInfo.GetOnlineCanSee(p, data.Rank).Count, Group.BannedRank.Players.Count);
            p.Message("  &a{0} &Slevels total (&a{1} &Sloaded). Currency is &3{2}&S.",
                      LevelInfo.AllMapFiles().Length, LevelInfo.Loaded.Count, Server.Config.Currency);

            TimeSpan up = DateTime.UtcNow - Server.StartTime;
            p.Message("  Been up for &a{0}&S, running &b{1} &a{2} &f" + Updater.SourceURL,
                      up.Shorten(true), Server.SoftwareName, Server.Version);

            int updateInterval = 1000 / Server.Config.PositionUpdateInterval;
            p.Message("  Player positions are updated &a{0} &Stimes/second", updateInterval);

            string owner = Server.Config.OwnerName;
            if (!owner.CaselessEq("Notch") && !owner.CaselessEq("the owner")) {
                p.Message("  Owner is &3{0}", owner);
            }

            if (HasExtraPerm(p, data.Rank, 1)) OutputResourceUsage(p);
        }

        static DateTime startTime;
        static TimeSpan startCPU;

        static void OutputResourceUsage(Player p) {
            Process proc = Process.GetCurrentProcess();
            p.Message("Measuring resource usage...one second");

            if (startTime == default(DateTime)) {
                startTime = DateTime.UtcNow;
                startCPU  = proc.TotalProcessorTime;
            }

            CPUTime beg     = MeasureAllCPUTime();
            TimeSpan begCPU = proc.TotalProcessorTime;

            // measure CPU usage over one second
            Thread.Sleep(1000);
            TimeSpan endCPU = proc.TotalProcessorTime;
            CPUTime end     = MeasureAllCPUTime();

            p.Message("&a{0}% &SCPU usage now, &a{1}% &Soverall",
                MeasureCPU(begCPU,   endCPU, TimeSpan.FromSeconds(1)),
                MeasureCPU(startCPU, endCPU, DateTime.UtcNow - startTime));

            ulong idl  = (end.IdleTime - beg.IdleTime);
            ulong sys  = (end.UserTime - beg.UserTime) + (end.KernelTime - beg.KernelTime);
            double cpu = sys * 100.0 / (sys + idl);
            p.Message("  &a{0}% &Sby all processes", 
                double.IsNaN(cpu) ? "(unknown)" : cpu.ToString("F2"));

            // Private Bytes = memory the process has reserved just for itself
            int memory = (int)Math.Round(proc.PrivateMemorySize64 / 1048576.0);
            p.Message("&a{0} &Sthreads, using &a{1} &Smegabytes of memory",
                proc.Threads.Count, memory);
        }

        static string MeasureCPU(TimeSpan beg, TimeSpan end, TimeSpan interval) {
            if (end < beg) return "0.00"; // TODO: Can this ever happen
            int cores = Math.Max(1, Environment.ProcessorCount);

            TimeSpan used  = end - beg;
            double elapsed = 100.0 * (used.TotalSeconds / interval.TotalSeconds);
            return (elapsed / cores).ToString("F2");
        }

        struct CPUTime
        {
            public ulong IdleTime, KernelTime, UserTime;
        }

        unsafe static CPUTime MeasureAllCPUTime() {
            switch (Server.DetectOS())
            {
                case DetectedOS.Windows: return MeasureAllWindows();
                case DetectedOS.OSX:     return MeasureAllMac();
                case DetectedOS.Linux:   return MeasureAllLinux();
            }
            return default(CPUTime);
        }


        static CPUTime MeasureAllWindows() {
            CPUTime all = default(CPUTime);
            GetSystemTimes(out all.IdleTime, out all.KernelTime, out all.UserTime);

            // https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getsystemtimes
            // lpKernelTime - "... This time value also includes the amount of time the system has been idle."
            all.KernelTime -= all.IdleTime;
            return all;
        }

        [DllImport("kernel32.dll")]
        static extern int GetSystemTimes(out ulong idleTime, out ulong kernelTime, out ulong userTime);


        // https://stackoverflow.com/questions/20471920/how-to-get-total-cpu-idle-time-in-objective-c-c-on-os-x
        // /usr/include/mach/host_info.h, /usr/include/mach/machine.h, /usr/include/mach/mach_host.h
        static CPUTime MeasureAllMac() {
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


        static CPUTime MeasureAllLinux() {
            // https://stackoverflow.com/questions/15145241/is-there-an-equivalent-to-the-windows-getsystemtimes-function-in-linux
            try {
                using (StreamReader r = new StreamReader("/proc/stat"))
                {
                    string line = r.ReadLine();
                    if (line.StartsWith("cpu ")) return ParseCpuLine(line);
                }
            } catch (FileNotFoundException) { }

            return default(CPUTime);
        }

        static CPUTime ParseCpuLine(string line)
        {
            string[] bits = line.SplitSpaces();

            ulong user = ulong.Parse(bits[2]);
            ulong nice = ulong.Parse(bits[3]);
            ulong kern = ulong.Parse(bits[4]);
            ulong idle = ulong.Parse(bits[5]);
            // TODO interrupt time too?

            CPUTime all;
            all.UserTime   = user + nice;
            all.KernelTime = kern;
            all.IdleTime   = idle;
            return all;
        }

        public override void Help(Player p) {
            p.Message("&T/ServerInfo");
            p.Message("&HDisplays the server information.");
        }
    }
}
