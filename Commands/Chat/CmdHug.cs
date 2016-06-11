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

namespace MCGalaxy.Commands {
    
    public class CmdHug : Command {
        public override string name { get { return "hug"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.name == p.name) {
                Player.Message(p, "You cannot hug yourself, silly!"); return;
            }
            if (p != null && p.muted) { Player.Message(p, "Cannot use /hug while muted."); return; }
            
            string giver = (p == null) ? "(console)" : p.ColoredName, type = null;
            if (args.Length > 1) {
                args[1] = args[1].ToLower();
                if (args[1] == "loving" || args[1] == "creepy" || args[1] == "friendly" || args[1] == "deadly")
                    type = args[1];
            }
            if (type == null) {
                Player.GlobalMessage(p, giver + " %Shugged " + who.ColoredName + "."); return;
            }
            
            if (type == "deadly") {
                if (p != null && p.group.Permission < LevelPermission.Operator) {
                    Player.Message(p, "You cannot %cdeath-hug %Sat your current rank."); return;
                }
                if (p != null && who.group.Permission > p.group.Permission) {
                    MessageTooHighRank(p, "&cdeath-hug%S", true); return;
                }
                who.HandleDeath(Block.rock, " died from a %cdeadly hug.");
            }
            Player.GlobalMessage(p, giver + " %Sgave " + who.ColoredName + " %Sa " + type + " hug.");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/hug [player] <type>");
            Player.Message(p, "%HValid types are: &floving, friendly, creepy and deadly.");
            Player.Message(p, "%HSpecifying no type or a non-existent type results in a normal hug.");
        }
    }
}
