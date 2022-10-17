/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.Commands.Maintenance { 
    public sealed class CmdBlockSpeed : Command2 {        
        public override string name { get { return "BlockSpeed"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string text, CommandData data) {
            if (text.Length == 0) { SendEstimation(p, data.Rank); return; }            
            string[] args = text.SplitSpaces();
            string cmd    = args[0].ToLower();

            if (args.Length == 1) { Help(p); return; }
            int value = 0;
            
            if (cmd == "updates" || cmd == "bs") { // "bs" was old option name
                if (!CommandParser.GetInt(p, args[1], "Blocks per interval", ref value, 0)) return;
                
                BlockQueue.UpdatesPerTick = value;
                p.Message("Blocks per interval is now {0}", BlockQueue.UpdatesPerTick);
            } else if (cmd == "interval" || cmd == "ts") { // "ts" was old option name
                if (!CommandParser.GetInt(p, args[1], "Block interval", ref value, 50)) return;
                
                BlockQueue.Interval = value;
                p.Message("Block interval is now {0}", BlockQueue.Interval);
            } else {
                Help(p); return;
            }
            SendEstimation(p, data.Rank);
        }
        
        static void SendEstimation(Player p, LevelPermission plRank) {
            int updates = BlockQueue.UpdatesPerTick, interval = BlockQueue.Interval;
            int count   = PlayerInfo.GetOnlineCanSee(p, plRank).Count;
            int blocksPerSec = updates * (1000 / interval);
            
            p.Message("{0} blocks every {1} milliseconds = {2} blocks per second", 
                     updates, interval, blocksPerSec);
            p.Message("  Using ~{0}KB/s times {1} players = ~{2}KB/s", 
                     (blocksPerSec * 8) / 1000, count, (count * blocksPerSec * 8) / 1000);
        }
        
        public override void Help(Player p) {
            p.Message("&T/BlockSpeed updates [value]");
            p.Message("&HSets max number of blocks that can be sent in one block update");
            p.Message("&T/BlockSpeed interval [value]");
            p.Message("&HSets the interval (in milliseconds) between block updates");
            p.Message("  &HMax blocks sent per second = updates * (1000 / interval)");
        }
    }
}
