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
    public sealed class CmdServerInfo : Command {
        public override string name { get { return "serverinfo"; } }
        public override string shortcut { get { return "sinfo"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("host"), new CommandAlias("zall") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can see server CPU and memory usage") }; }
        }
        
        static PerformanceCounter allPCounter = null;
        static PerformanceCounter cpuPCounter = null;
        
        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            
            Player.Message(p, "Server's name: &b{0}%S", ServerConfig.Name);
            Player.Message(p, "&a{0} %Splayers total. (&a{1} %Sonline, &8{2} banned%S)",
                           GetPlayerCount(), PlayerInfo.Online.Count, Group.BannedRank.Players.Count);
            Player.Message(p, "&a{0} %Slevels currently loaded. Currency is &3{1}%S.",
                           LevelInfo.Loaded.Count, ServerConfig.Currency);
            
            TimeSpan up = DateTime.UtcNow - Server.StartTime;
            Player.Message(p, "Been up for &b{0}%S, running &b{1} &a{2} %S(based on &bMCForge %Sand &bMCLawl%S).",
                           up.Shorten(true), Server.SoftwareName, Server.VersionString);

            Player.Message(p, "Player positions are updated every &b"
                           + ServerConfig.PositionUpdateInterval + " %Smilliseconds.");
            string owner = ServerConfig.OwnerName;
            if (!owner.CaselessEq("Notch"))
                Player.Message(p, "Owner is &3{0}. %SConsole state: &3{1}", owner, ServerConfig.ConsoleName);
            else
                Player.Message(p, "Console state: &3{0}", ServerConfig.ConsoleName);
            
            if (CheckExtraPerm(p))
                ShowServerStats(p);
        }
        
        static int GetPlayerCount() {
            // Use fast path if possible  TODO: fast path for mysql
            int count = 0;
            if (!ServerConfig.UseMySQL) {
                DataTable maxTable = Database.Backend.GetRows("Players", "MAX(_ROWID_)", "LIMIT 1");
                if (maxTable.Rows.Count > 0) {
                     string row = maxTable.Rows[0]["MAX(_ROWID_)"].ToString();
                     maxTable.Dispose();
                     if (int.TryParse(row, out count) && count > 0) return count;
                }             
            }
            
            DataTable table = Database.Backend.GetRows("Players", "COUNT(id)");
            count = int.Parse(table.Rows[0]["COUNT(id)"].ToString());
            table.Dispose();
            return count;
        }
        
        void ShowServerStats(Player p) {
            Process proc = Process.GetCurrentProcess();
            if (allPCounter == null) {
                Player.Message(p, "Starting performance counters...one second");
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
            Player.Message(p, "&a{0}% %SCPU usage, &a{1}% %Sby all processes", cpu.ToString("F2"), all.ToString("F2"));
            Player.Message(p, "&a{0} %Sthreads, using &a{1} %Smegabytes of memory", threads, mem);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/serverinfo");
            Player.Message(p, "%HDisplays the server information.");
        }
    }
}
