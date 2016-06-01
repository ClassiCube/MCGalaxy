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
            
            // TODO: use max rowid to be faster
            DataTable table = Database.fillData("SELECT COUNT(id) FROM Players");
            int count = int.Parse(table.Rows[0]["COUNT(id)"].ToString());
            table.Dispose();
            
            Player.Message(p, "Server's name: &b{0}%S", Server.name);
            Player.Message(p, "&a{0}%S players total. (&a{1}%S online, &8{2} banned%S)",
                           count, Player.number, Player.GetBannedCount());
            Player.Message(p, "&a{0} %Slevels currently loaded. Currency is &3{1}%S.",
                           LevelInfo.Loaded.Count, Server.moneys);
            
            TimeSpan up = DateTime.UtcNow - Server.StartTime;
            Player.Message(p, "Been up for &b" + WhoInfo.Shorten(up, true) +
                           "%S, running &bMCGalaxy &a" + Server.VersionString +
                           "%S (based on &bMCForge %Sand &bMCLawl%S).");

            Player.Message(p, "Player positions are updated every &b"
                           + Server.updateTimer.Interval + " %Smilliseconds.");
            string owner = Server.server_owner;
            if (!owner.CaselessEq("Notch"))
                Player.Message(p, "Owner is &3{0}. %SConsole state: &3{1}", owner, Server.ZallState);
            else
                Player.Message(p, "Console state: &3{0}", Server.ZallState);
            
            if (CheckExtraPerm(p))
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
                System.Threading.Thread.Sleep(500);
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
