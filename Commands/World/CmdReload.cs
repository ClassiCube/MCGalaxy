/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Commands {
    
    public sealed class CmdReload : Command {
        
        public override string name { get { return "reload"; } }
        public override string shortcut { get { return "rd"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can reload the maps for others", 1) }; }
        }

        public override void Use(Player p, string message) {
            if (p == null && message == "") {
                Player.SendMessage(p, "You must give a level name when using the command from console."); return;
            }            
            if (message == "") { CmdReveal.ReloadMap(p, p, false); return; }
            
            int minPerm = CommandOtherPerms.GetPerm(this, 1);
            if (p != null && (int)p.group.Permission < minPerm) {
                Player.SendMessage(p, "%HYou need to be at least %T" + Group.findPermInt(minPerm).name + " %Hto reload maps."); return;
            }            
            if (!LevelInfo.ExistsOffline(message)) {
                Player.SendMessage(p, "The given level \"" + message + "\" does not exist."); return;
            } 
            
            Player[] players = PlayerInfo.Online;
            foreach (Player pl in players) {
                if (pl.level.name.ToLower() != message.ToLower()) continue;
                CmdReveal.ReloadMap(p, pl, false);
            }            
            Player.GlobalMessage("&cThe map, " + message + " has been reloaded!");
            Server.IRC.Say("The map, " + message + " has been reloaded.");
            string src = p == null ? "the console" : p.name;
            Server.s.Log("The map " + message + " was reloaded by " + src);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/reload [map]");
            Player.SendMessage(p, "%HReloads the given map for all players in that map.");
            Player.SendMessage(p, "%HIf no map is specified, reloads the map you are in just for you.");
            int minPerm = CommandOtherPerms.GetPerm(this, 1);
            Player.SendMessage(p, "%T" + Group.findPermInt(minPerm).name + " %H or above can reload maps for all players.");
        }
    }
}
