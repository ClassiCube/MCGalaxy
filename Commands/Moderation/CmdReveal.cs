/*
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdReveal : Command {
        
        public override string name { get { return "reveal"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can reveal to everyone") }; }
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
                if (lvl == null) {
                    Player.SendMessage(p, "Level not found."); return;
                }            
                if (p != null && (int)p.group.Permission < CommandOtherPerms.GetPerm(this)) { 
                    Player.SendMessage(p, "Reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + "+"); return; 
                }

            	Player[] players = PlayerInfo.Online;
                foreach (Player who in players) {
                    if (who.level == lvl)
                        ReloadMap(p, who, true);
                }
            } else {
                Player who = PlayerInfo.Find(parts[0]);
                if (who == null) { 
                    Player.SendMessage(p, "Could not find player."); return; 
                } else if (who.group.Permission > p.group.Permission && p != who) { 
                    Player.SendMessage(p, "Cannot reload the map of someone higher than you."); return; 
                }        
                ReloadMap(p, who, true);        
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        internal static void ReloadMap(Player p, Player who, bool showMessage) {
            who.Loading = true;
            Player[] players = PlayerInfo.Online;
            foreach (Player pl in players) if (who.level == pl.level && who != pl) who.SendDespawn(pl.id);
            foreach (PlayerBot b in PlayerBot.playerbots.ToArray()) if (who.level == b.level) who.SendDespawn(b.id);

            ushort x = who.pos[0], y = who.pos[1], z = who.pos[2];
            Player.GlobalDespawn(who, true);
            who.SendUserMOTD(); who.SendMap(who.level);

            if (!who.hidden)
                Player.GlobalSpawn(who, x, y, z, who.level.rotx, who.level.roty, true);
            else
                who.SendPos(0xFF, x, y, z, who.level.rotx, who.level.roty);

            players = PlayerInfo.Online;
            foreach (Player pl in players)
                if (pl.level == who.level && who != pl && !pl.hidden)
                    who.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);

            foreach (PlayerBot b in PlayerBot.playerbots.ToArray())
                if (b.level == who.level)
                    who.SendSpawn(b.id, b.color + b.name, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);

            who.Loading = false;

            if (!showMessage) return;
            if (p != null && !p.hidden) { who.SendMessage("&bMap reloaded by " + p.name); }
            if (p != null && p.hidden) { who.SendMessage("&bMap reloaded"); }
            Player.SendMessage(p, "&4Finished reloading for " + who.name);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/reveal <name> - Reveals the map for <name>.");
            Player.SendMessage(p, "/reveal all - Reveals for all in the map");
            Player.SendMessage(p, "/reveal all <map> - Reveals for all in <map>");
            Player.SendMessage(p, "Will reload the map for anyone. (incl. banned)");
        }
    }
}
