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

namespace MCGalaxy.Commands {    
    public sealed class CmdBlockSpeed : Command {        
        public override string name { get { return "blockspeed"; } }
        public override string shortcut { get { return "bs"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string text) {
            if (text == "") { SendEstimation(p); return; }            
            string[] args = text.SplitSpaces();
            string cmd = args[0].ToLower();
            if (cmd == "clear") {
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    lock (lvl.queueLock)
                        lvl.blockqueue.Clear();
                }
                return;
            }
            if (args.Length == 1) { Help(p); return; }
            int value = 0;
            
            if (cmd == "bs") {
                if (!CommandParser.GetInt(p, args[1], "Blocks per interval", ref value, 0)) return;
                
                BlockQueue.blockupdates = value;
                Player.Message(p, "Blocks per interval is now {0}.", BlockQueue.blockupdates);
            } else if (cmd == "ts") {
                if (!CommandParser.GetInt(p, args[1], "Block interval", ref value, 50)) return;
                
                BlockQueue.time = value;
                Player.Message(p, "Block interval is now {0}.", BlockQueue.time);
            } else if (cmd == "buf")  {
                if (p.level.bufferblocks)
                    Player.Message(p, "Block buffering on {0} %Sdisabled.", p.level.ColoredName);
                else
                    Player.Message(p, "Block buffering on {0} %Senabled.", p.level.ColoredName);
                p.level.bufferblocks = !p.level.bufferblocks;
            } else if (cmd == "net") {
                if (!CommandParser.GetInt(p, args[1], "value", ref value, 2, 1000)) return;
                
                switch (value) {
                    case 2: Set(25, 100); break;
                    case 4: Set(50, 100); break;
                    case 8: Set(100, 100); break;
                    case 12: Set(200, 100); break;
                    case 16: Set(200, 100); break;
                    case 161: Set(100, 50); break;
                    case 20: Set(125, 50); break;
                    case 24: Set(150, 50); break;
                    default: Set(200, 100); break;
                }
                SendEstimation(p);
            }
        }
        
        static void Set(int updates, int time) {
            BlockQueue.blockupdates = updates;
            BlockQueue.time = time;
        }
        
        static void SendEstimation(Player p) {
            int updates = BlockQueue.blockupdates, time = BlockQueue.time, count = PlayerInfo.Online.Count;
            Player.Message(p, "{0} blocks every {1} milliseconds = {2} blocks per second.", updates, time, updates * (1000 / time));
            Player.Message(p, "Using ~{0}KB/s times {1} player(s) = ~{2}KB/s", (updates * (1000 / time) * 8) / 1000, count, count * ((updates * (1000 / time) * 8) / 1000));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/bs [option] [value].");
            Player.Message(p, "%HOptions: %Sbs (blocks per interval), ts (interval in milliseconds), buf (toggles buffering), clear");
            Player.Message(p, "%T/bs net [2,4,8,12,16,20,24]");
            Player.Message(p, "%HPresets, divide by 8 and times by 1000 to get blocks per second.");
        }
    }
}
