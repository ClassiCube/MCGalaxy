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

namespace MCGalaxy.Commands {
    public sealed class CmdServerInfo : Command {
        public override string name { get { return "serverinfo"; } }
        public override string shortcut { get { return "sinfo"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdServerInfo() { }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("host"), new CommandAlias("zall") }; }
        }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can see server CPU and memory usage") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            
            Player.Message(p, "Server's name: &b" + Server.name + "%S");
            Player.Message(p, "&a" + Player.number + " %Splayers online, &8"
                           + Player.GetBannedCount() + " banned%S players total.");
            Player.Message(p, "&a" + LevelInfo.Loaded.Count + " %Slevels currently loaded. " +
                           "Currency is &3" + Server.moneys + "%S.");
            
            TimeSpan up = DateTime.UtcNow - Server.StartTime;
            Player.Message(p, "Been up for &b" + WhoInfo.Shorten(up, true) +
                           "%S, and is running &bMCGalaxy &a" + Server.VersionString +
                           "%S (based on &bMCForge %Sand &bMCLawl%S).");
            Command.all.Find("devs").Use(p, "");

            Player.Message(p, "Player positions are updated every &b"
                           + Server.updateTimer.Interval + " %Smilliseconds.");
            string owner = Server.server_owner;
            if (!owner.CaselessEq("Notch"))
                Player.Message(p, "Owner is &3" + owner + ". %SConsole state: &3" + Server.ZallState);
            else
                Player.Message(p, "Console state: &3" + Server.ZallState);
            
            if (CheckAdditionalPerm(p))
                ShowServerStats(p);
        }
        
        void ShowServerStats(Player p) {
            Process proc = Process.GetCurrentProcess();
            if (Server.PCCounter == null) {
                Player.Message(p, "Starting performance counters...one second");
                Server.PCCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                Server.PCCounter.BeginInit();
                Server.PCCounter.NextValue();

                Server.ProcessCounter = new PerformanceCounter("Process", "% Processor Time", proc.ProcessName);
                Server.ProcessCounter.BeginInit();
                Server.ProcessCounter.NextValue();
            }
            
            // Private Bytes because it is what the process has reserved for itself
            int threads = proc.Threads.Count;
            int mem = (int)Math.Round(proc.PrivateMemorySize64 / 1048576.0);
            double cpu = Server.ProcessCounter.NextValue(), all = Server.PCCounter.NextValue();
            Player.Message(p, "&a{0}% %SCPU usage, &a{1}% %Sby all processes", cpu.ToString("F2"), all.ToString("F2"));
            Player.Message(p, "&a{0}%S threads, using &a{1}%S megabytes of memory", threads, mem);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/serverinfo - Displays the server information.");
        }
    }
}
