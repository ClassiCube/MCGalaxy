/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
    
    public sealed class CmdSave : Command {
        
        public override string name { get { return "save"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("mapsave"), new CommandAlias("wsave") }; }
        }
        public CmdSave() { }

        public override void Use(Player p, string message) {
            if (message.ToLower() == "all") {
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level l in loaded) {
                    try {
                		if (l.ShouldSaveChanges()) l.Save();
                        else { Server.s.Log("Level \"" + l.name + "\" is running a game, skipping save."); }
                	} catch (Exception ex) {
                		Server.ErrorLog(ex);
                	}
                }
                Player.GlobalMessage("All levels have been saved.");
            } else {
                string[] args = message.Split(' ');
                if (message == "") {
                    if (p == null) Use(p, "all");
                    else Save(p, p.level, "");
                } else if (args.Length <= 2) {
                    Level lvl = LevelInfo.FindMatches(p, args[0]);
                    if (lvl == null) return;
                    string restore = args.Length > 1 ? args[1].ToLower() : "";
                    Save(p, lvl, restore);
                } else {
                    Help(p);
                }
            }
        }
        
        static void Save(Player p, Level lvl, string restoreName) {
            lvl.Save(true);
            Player.Message(p, "Level \"" + lvl.name + "\" saved.");
            int num = lvl.Backup(true, restoreName);
            if (num == -1) return;
            
            if (restoreName == "") {
                Server.s.Log("Backup " + num + " saved for " + lvl.name);
                Player.GlobalMessage("Backup " + num + " saved for " + lvl.name);
            } else {
                Server.s.Log(lvl.name + " had a backup created named &b" + restoreName);
                Player.GlobalMessage(lvl.name + " had a backup created named &b" + restoreName);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/save %H- Saves the level you are currently in");
            Player.Message(p, "%T/save all %H- Saves all loaded levels.");
            Player.Message(p, "%T/save <map> %H- Saves the specified map.");
            Player.Message(p, "%T/save <map> <name> %H- Backups the map with a given restore name");
        }
    }
}
