/*
    Written by Jack1312
    Copyright 2011 MCGalaxy
        
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
using System.IO;
namespace MCGalaxy.Commands {
    
    public sealed class CmdAllowGuns : Command {
        
        public override string name { get { return "allowguns"; } }
        public override string shortcut { get { return "ag"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdAllowGuns() { }

        public override void Use(Player p, string message) {
            Level lvl = null;
            if (message == "") {
                if (p == null) {
                    Player.SendMessage(p, "You must provide a level name when using this command from console."); return;
                }
                lvl = p.level;
            } else {
                lvl = Level.Find(message);
                if (lvl == null || !File.Exists("levels/" + message + ".lvl")) {
                    Player.SendMessage(p, "&9The level, &c" + message + " &9does not exist!"); return;
                }
            }
            
            if (lvl.guns) {
                Player.GlobalMessage("&9Gun usage has been disabled on &c" + lvl.name + "&9!");
                foreach (Player pl in PlayerInfo.players)
                    if (pl.level.name.ToLower() == lvl.name.ToLower())
                        pl.aiming = false;
            } else {
                Player.GlobalMessage("&9Gun usage has been enabled on &c" + lvl.name + "&9!");            
            }
            lvl.guns = !lvl.guns;
            Level.SaveSettings(lvl);
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/allowguns - Allow/disallow guns and missiles on the specified level. If no message is given, the current level is taken.");
            Player.SendMessage(p, "Note: If guns are allowed on a map, and /allowguns is used, all guns and missiles will be disabled.");
        }
    }
}