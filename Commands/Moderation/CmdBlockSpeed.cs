/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
            string[] args = text.Split(' ');
            string cmd = args[0].ToLower();
            if (cmd == "clear") {
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) 
                    lvl.blockqueue.Clear();
                return;
            }
            if (args.Length == 1) { Help(p); return; }
            int value = 0;
            
            if (cmd == "bs") {
            	if (!int.TryParse(args[1], out value)) { 
            		Player.Message(p, "Invalid number specified."); return; 
            	}
            	BlockQueue.blockupdates = value;
                Player.Message(p, String.Format("Blocks per interval is now {0}.", BlockQueue.blockupdates));
            } else if (cmd == "ts") {
                if (!int.TryParse(args[1], out value)) { 
            		Player.Message(p, "Invalid number specified."); return; 
            	}
            	BlockQueue.time = value;
                Player.Message(p, String.Format("Block interval is now {0}.", BlockQueue.time));
            } else if (cmd == "buf")  {
                if (p.level.bufferblocks)
                    Player.Message(p, String.Format("Block buffering on {0} disabled.", p.level.name));
                else
                    Player.Message(p, String.Format("Block buffering on {0} enabled.", p.level.name));
                p.level.bufferblocks = !p.level.bufferblocks;
            } else if (cmd == "net") {
            	if (!int.TryParse(args[1], out value)) { 
            		Player.Message(p, "Invalid number specified."); return; 
            	}
            	
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
            Player.Message(p, String.Format("{0} blocks every {1} milliseconds = {2} blocks per second.", BlockQueue.blockupdates, BlockQueue.time, BlockQueue.blockupdates * (1000 / BlockQueue.time)));
            Player.Message(p, String.Format("Using ~{0}KB/s times {1} player(s) = ~{2}KB/s", (BlockQueue.blockupdates * (1000 / BlockQueue.time) * 8) / 1000, PlayerInfo.Online.Count, PlayerInfo.Online.Count * ((BlockQueue.blockupdates * (1000 / BlockQueue.time) * 8) / 1000)));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/bs [option] [option value] - Options for block speeds.");
            Player.Message(p, "Options are: bs (blocks per interval), ts (interval in milliseconds), buf (toggles buffering), clear, net.");
            Player.Message(p, "/bs net [2,4,8,12,16,20,24] - Presets, divide by 8 and times by 1000 to get blocks per second.");
        }
    }
}
