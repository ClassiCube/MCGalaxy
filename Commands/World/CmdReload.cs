/*
    Copyright 2011 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System.IO;
namespace MCGalaxy.Commands {
	
    public sealed class CmdReload : Command {
		
        public override string name { get { return "reload"; } }
        public override string shortcut { get { return "rd"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdReload() { }

        public override void Use(Player p, string message) {
            if (p == null && message == "") {
                Player.SendMessage(p, "You must give a level name when running the command from console."); return;
            }
            
            string name = message == "" ? p.level.name : message;
            if (!File.Exists("levels/" + name + ".lvl")) {
                Player.SendMessage(p, "The given level \"" + name + "\" does not exist."); return;
            }
            if (Server.mainLevel.name == name) {
                Player.SendMessage(p, "You cannot reload the main level."); return;
            }
            
            foreach (Player pl in Player.players) {
                if (pl.level.name.ToLower() != name.ToLower()) continue;
                
                if (p != null)
                    p.ignorePermission = true;
                Command.all.Find("unload").Use(p, name);
                Command.all.Find("load").Use(p, name);
                Command.all.Find("goto").Use(pl, name);
            }
            
            Player.GlobalMessage("&cThe map, " + name + " has been reloaded!");
            Server.IRC.Say("The map, " + name + " has been reloaded.");
            string src = p == null ? "the console" : p.name;
            Server.s.Log("The map " + name + " was reloaded by " + src);
            if (p != null )
                p.ignorePermission = false;
            return;
        }
        
        public override void Help(Player p) {
        	Player.SendMessage(p, "%T/reload [map]");
        	Player.SendMessage(p, "%HReloads the given map. " +
        	                   "If no map is specified, reloads the current map you are in.");
        }
    }
}
