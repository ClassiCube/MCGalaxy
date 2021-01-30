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
using System.Data;
using System.Diagnostics;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdServerInfo : Command2 {
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
        
        static PerformanceCounter allPCounter = null;
        static PerformanceCounter cpuPCounter = null;

        public override void Use(Player p, string message, CommandData data) {
            int count = Database.CountRows("Players");
            p.Message("Server's name: &b{0}&S", Server.Config.Name);
            p.Message("&a{0} &Splayers total. (&a{1} &Sonline, &8{2} banned&S)",
                      count, PlayerInfo.Online.Count, Group.BannedRank.Players.Count);
            p.Message("&a{0} &Slevels currently loaded. Currency is &3{1}&S.",
                      LevelInfo.Loaded.Count, Server.Config.Currency);
            
            TimeSpan up = DateTime.UtcNow - Server.StartTime;
            p.Message("Been up for &b{0}&S, running &b{1} &a{2} &f" + Updater.SourceURL,
                      up.Shorten(true), Server.SoftwareName, Server.Version);
            p.Message("Player positions are updated every &b"
                      + Server.Config.PositionUpdateInterval + " &Smilliseconds.");
            
            string owner = Server.Config.OwnerName;
            if (!owner.CaselessEq("Notch") && !owner.CaselessEq("the owner")) {
                p.Message("Owner is &3{0}. &SConsole state: &3{1}", owner, Server.Config.ConsoleName);
            } else {
                p.Message("Console state: &3{0}", Server.Config.ConsoleName);
            }
            
            if (HasExtraPerm(p, data.Rank, 1)) ShowServerStats(p);
        }

        void ShowServerStats(Player p) {
            Process proc = Process.GetCurrentProcess();
            if (allPCounter == null) {
                p.Message("Starting performance counters...one second");
                allPCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                allPCounter.BeginInit();
                allPCounter.NextValue();

                cpuPCounter = new PerformanceCounter("Process", "% Processor Time", proc.ProcessName);
                cpuPCounter.BeginInit();
                cpuPCounter.NextValue();
                System.Threading.Thread.Sleep(500);
            }
            
            // Private Bytes because it is what the process has reserved for itself
            int threads = proc.Threads.Count;
            int mem = (int)Math.Round(proc.PrivateMemorySize64 / 1048576.0);
            double cpu = cpuPCounter.NextValue(), all = allPCounter.NextValue();
            p.Message("&a{0}% &SCPU usage, &a{1}% &Sby all processes", cpu.ToString("F2"), all.ToString("F2"));
            p.Message("&a{0} &Sthreads, using &a{1} &Smegabytes of memory", threads, mem);
        }
        
        public override void Help(Player p) {
            p.Message("&T/ServerInfo");
            p.Message("&HDisplays the server information.");
        }
    }
}
