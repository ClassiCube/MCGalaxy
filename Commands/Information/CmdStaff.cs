/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdStaff : Command {
        public override string name { get { return "staff"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdStaff() { }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            Command.all.Find("devs").Use(p, "");
            Command.all.Find("mods").Use(p, "");
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/staff - Displays the MCGalaxy team.");
        }
    }
    
     public sealed class CmdMods : Command {
        
        public override string name { get { return "mods"; } }
        public override string shortcut { get { return "mod"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            string modlist = string.Join(", ", Server.Mods);
            Player.SendMessage(p, "&9MCGalaxy mods: %S" + modlist + "&e.");
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/mods - Displays the list of MCGalaxy moderators.");
        }
    }
    
    public sealed class CmdDevs : Command {
        
        public override string name { get { return "devs"; } }
        public override string shortcut { get { return "dev"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            string devlist = string.Join(", ", Server.Devs);
            Player.SendMessage(p, "&9MCGalaxy devs: %S" + devlist + "&e.");
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/devs - Displays the list of MCGalaxy developers.");
        }
    }
}
