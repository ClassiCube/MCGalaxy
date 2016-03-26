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
using System.Collections.Generic;
using System.Threading;

namespace MCGalaxy.Commands {
    
    public sealed class CmdMapSet : Command {
        public override string name { get { return "mapset"; } }
        public override string shortcut { get { return "mset"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool Enabled { get { return Server.ZombieModeOn || Server.lava.active; } }
        static char[] trimChars = {' '};
        
        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") {
                Player.SendMessage(p, "Map authors: " + p.level.Authors);
                Player.SendMessage(p, "Pillaring allowed: " + p.level.Pillaring);
                Player.SendMessage(p, "Build type: " + p.level.BuildType);
                return;
            }
            
            string[] args = message.Split(trimChars, 2);
            if (args.Length == 1) { Player.SendMessage(p, "You need to provide a value."); return; }
            
            if (args[0].CaselessEq("author") || args[0].CaselessEq("authors")) {
                p.level.Authors = args[1].Replace(" ", "%S, ");
                Player.SendMessage(p, "Sets the authors of the map to: " + args[1]);
            } else if (args[0].CaselessEq("pillar") || args[0].CaselessEq("pillaring")) {
                bool value;
                if (!bool.TryParse(args[1], out value)) {
                    Player.SendMessage(p, "Value must be 'true' or 'false'"); return;
                }
                p.level.Pillaring = value;
                Player.SendMessage(p, "Set pillaring allowed to: " + value);
            } else if (args[0].CaselessEq("build") || args[0].CaselessEq("buildtype")) {
                BuildType value;
                if (!Enum.TryParse(args[1], true, out value)) {
                    Player.SendMessage(p, "Value must be 'normal', 'modifyonly', or 'momodify'"); return;
                }
                p.level.BuildType = value;
                Player.SendMessage(p, "Set build type to: " + value);
            } else if (args[0].CaselessEq("minroundtime") || args[0].CaselessEq("minround")) {
                byte time;
                if (!byte.TryParse(args[1], out time) || time == 0 || time > 10) {
                    Player.SendMessage(p, "Minutes must be an integer between 1 and 10."); return;
                }
                
                if (time > p.level.MaxRoundTime) {
                    Player.SendMessage(p, "Min round time must be less than or equal to max round time"); return;
                }
                p.level.MinRoundTime = time;
                Player.SendMessage(p, "Set min round time to: " + time + " minutes");
            } else if (args[0].CaselessEq("maxroundtime") || args[0].CaselessEq("maxround")) {
                byte time;
                if (!byte.TryParse(args[1], out time) || time == 0 || time > 10) {
                    Player.SendMessage(p, "Minutes must be an integer between 1 and 10."); return;
                }
                
                if (time < p.level.MinRoundTime) {
                    Player.SendMessage(p, "Max round time must be greater than or equal to min round time"); return;
                }
                p.level.MaxRoundTime = time;
                Player.SendMessage(p, "Set max round time to: " + time + " minutes");
            } else {
                Player.SendMessage(p, "Unrecognised property \"" + args[0] + "\"."); return;
            }
            Level.SaveSettings(p.level);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%HThis sets the various options for games on this map.");
            Player.SendMessage(p, "%T/mapset authors [name1] <name2> <name3>...");
            Player.SendMessage(p, "%HThis is shown to players at the start of rounds.");
            Player.SendMessage(p, "%T/mapset pillaring [true/false]");
            Player.SendMessage(p, "%T/mapset build [normal/modifyonly/nomodify]");
            Player.SendMessage(p, "%T/mapset minroundtime [minutes]");
            Player.SendMessage(p, "%T/mapset maxroundtime [minutes]");
        }
    }
}
