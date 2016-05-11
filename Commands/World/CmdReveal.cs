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
    
    public sealed class CmdReveal : Command {
        
        public override string name { get { return "reveal"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "Lowest rank that can reveal to everyone") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") message = p.name;
            string[] parts = message.Split(' ');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].ToLower();
            
            Level lvl = null;
            if (parts.Length == 2) {
                lvl = LevelInfo.Find(parts[1]);
            } else if (p != null && p.level != null) {
                lvl = p.level;
            }

            if (parts[0] == "all") {
                if (lvl == null) { Player.Message(p, "Level not found."); return; }            
                if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can reload all players in a map."); return; }

                Player[] players = PlayerInfo.Online.Items;
                foreach (Player who in players) {
                    if (who.level == lvl)
                        ReloadMap(p, who, true);
                }
            } else {
                Player who = PlayerInfo.FindOrShowMatches(p, parts[0]);
                if (who == null) { 
                    return;
                } else if (who.group.Permission > p.group.Permission && p != who) {
                    MessageTooHighRank(p, "reload the map for", true); return;
                }        
                ReloadMap(p, who, true);        
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        internal static void ReloadMap(Player p, Player who, bool showMessage) {
            who.Loading = true;
            Entities.DespawnEntities(who);
            who.SendUserMOTD(); who.SendMap(who.level);            
            Entities.SpawnEntities(who);
            who.Loading = false;

            if (!showMessage) return;
            if (p != null && !p.hidden) { who.SendMessage("&bMap reloaded by " + p.name); }
            if (p != null && p.hidden) { who.SendMessage("&bMap reloaded"); }
            Player.Message(p, "&4Finished reloading for " + who.name);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/reveal <name> - Reloads the map for <name>.");
            Player.Message(p, "/reveal all - Reloads for all players in your map");
            Player.Message(p, "/reveal all <map> - Reloads for all players in <map>");
        }
    }
    
    public sealed class CmdReload : Command {
        
        public override string name { get { return "reload"; } }
        public override string shortcut { get { return "rd"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
		public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("rejoin") }; }
		}

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            CmdReveal.ReloadMap(p, p, false);    
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/reload");
            Player.Message(p, "%HReloads the map you in, just for you.");
            Player.Message(p, "%HUse %T/reveal %Hto reload maps for other players.");
        }
    }
}
