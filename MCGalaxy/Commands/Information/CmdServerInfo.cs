/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
            IOperatingSystem os = IOperatingSystem.DetectOS();

            if (startTime == default(DateTime)) {
                startTime = DateTime.UtcNow;
                startCPU  = proc.TotalProcessorTime;
            }

            CPUTime beg     = os.MeasureAllCPUTime();
            TimeSpan begCPU = proc.TotalProcessorTime;

            // measure CPU usage over one second
            Thread.Sleep(1000);
            TimeSpan endCPU = proc.TotalProcessorTime;
            CPUTime end     = os.MeasureAllCPUTime();

            p.Message("&a{0}% &SCPU usage now, &a{1}% &Soverall",
                MeasureCPU(begCPU,   endCPU, TimeSpan.FromSeconds(1)),
                MeasureCPU(startCPU, endCPU, DateTime.UtcNow - startTime));

            ulong idl  = (end.IdleTime - beg.IdleTime);
            ulong sys  = (end.UserTime - beg.UserTime) + (end.KernelTime - beg.KernelTime);
            double cpu = sys * 100.0 / (sys + idl);
            int cores  = Environment.ProcessorCount;
            p.Message("  &a{0}% &Sby all processes across {1} CPU core{2}", 
                double.IsNaN(cpu) ? "(unknown)" : cpu.ToString("F2"),
                cores, cores.Plural());

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

        public override void Help(Player p) {
            p.Message("&T/ServerInfo");
            p.Message("&HDisplays the server information.");
        }
    }
}
