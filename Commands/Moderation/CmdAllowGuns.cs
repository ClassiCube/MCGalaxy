/*
    Written by Jack1312
    Copyright 2011 MCForge
        
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
            if (CheckSuper(p, message, "level name")) return;
            Level lvl = p.level;
            if (message != "") {
                lvl = LevelInfo.FindMatches(p, message);
                if (lvl == null) return;
            }
            
            if (lvl.guns) {
                Player.GlobalMessage("&9Gun usage has been disabled on &c" + lvl.name + "&9!");
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players)
                	if (pl.level.name.CaselessEq(lvl.name))
                        pl.aiming = false;
            } else {
                Player.GlobalMessage("&9Gun usage has been enabled on &c" + lvl.name + "&9!");
            }
            lvl.guns = !lvl.guns;
            Level.SaveSettings(lvl);
        }

        public override void Help(Player p) {
            Player.Message(p, "/allowguns [level]");
            Player.Message(p, "%HAllows/disallows guns and missiles on the specified level.");
            Player.Message(p, "%HIf no [level] is given, uses your current level.");
        }
    }
}