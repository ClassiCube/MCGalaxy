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
namespace MCGalaxy.Commands.World {
    public sealed class CmdReload : Command {
        public override string name { get { return "reload"; } }
        public override string shortcut { get { return "reveal"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("rejoin"), new CommandAlias("rd"), 
                    new CommandAlias("wflush"), new CommandAlias("worldflush") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can reload for all players") }; }
        }

        public override void Use(Player p, string message)
        {
            if (CheckSuper(p, message, "player or level name")) return;
            if (message == "") message = p.name;
            string[] parts = message.ToLower().SplitSpaces();
            
            if (parts[0] == "all") {
                if (!ReloadAll(p, parts)) return;
            } else {
                Player who = PlayerInfo.FindMatches(p, parts[0]);
                if (who == null) return;
                if (p != null && who.Rank > p.Rank) {
                    MessageTooHighRank(p, "reload the map for", true); return;
                }
                LevelActions.ReloadMap(p, who, true);
            }
            Server.DoGC();
        }
        
        bool ReloadAll(Player p, string[] parts) {
            Level lvl = null;
            if (parts.Length == 2) {
                lvl = Matcher.FindLevels(p, parts[1]);
                if (lvl == null) return false;
            } else if (!Player.IsSuper(p)) {
                lvl = p.level;
            } else {
                SuperRequiresArgs(name + " all", p, "level name"); return false;
            }
            
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, 1); return false; }
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player who in players) {
                if (who.level == lvl)
                    LevelActions.ReloadMap(p, who, true);
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/reload %H- Reloads the map you are in, just for you.");
            Player.Message(p, "%T/reload [name] %H- Reloads the map for [name].");
            Player.Message(p, "%T/reload all %H- Reloads for all players in map you are in.");
            Player.Message(p, "%T/reload all [map] %H- Reloads for all players in [map]");
        }
    }
}
