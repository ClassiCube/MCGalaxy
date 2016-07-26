/*
    Copyright 2011 MCForge
    
    Written by GamezGalaxy (hypereddie10)
    
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
namespace MCGalaxy.Commands {
    public sealed class CmdHigh5 : Command {
        public override string name { get { return "high5"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdHigh5() { }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            if (p != null && p.muted) { Player.Message(p, "Cannot use /high5 while muted."); return; }

            string giver = (p == null) ? "(console)" : p.ColoredName;
            Player.Message(who, giver + " just highfived you");
            Player.GlobalMessage(p, giver + " %Sjust highfived " + who.ColoredName);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/high5 <player>");
            Player.Message(p, "%HHigh five someone :D");
        }
    }
}
