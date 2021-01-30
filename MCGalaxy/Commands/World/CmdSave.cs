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

namespace MCGalaxy.Commands.World {
    public sealed class CmdSave : Command2 {
        
        public override string name { get { return "Save"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("MapSave"), new CommandAlias("WSave"), new CommandAlias("WorldSave") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.CaselessEq("all")) { SaveAll(p); return; }
            if (message.Length == 0) {
                if (p.IsSuper) { SaveAll(p); }
                else { Save(p, p.level, ""); }
                return;
            }
            
            string[] args = message.SplitSpaces();
            if (args.Length <= 2) {
                Level lvl = Matcher.FindLevels(p, args[0]);
                if (lvl == null) return;
                
                string restore = args.Length > 1 ? args[1].ToLower() : "";
                Save(p, lvl, restore);
            } else {
                Help(p);
            }
        }
        
        static void SaveAll(Player p) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                TrySave(p, lvl, false);
            }
            Chat.MessageGlobal("All levels have been saved.");
        }
        
        static bool TrySave(Player p, Level lvl, bool force) {
            if (!force && !lvl.Changed) return false;
            
            if (!lvl.SaveChanges) {
                p.Message("Level {0} &Sis running a game, skipping save", lvl.ColoredName);
                return false;
            }
            
            bool saved = lvl.Save(force);
            if (!saved) p.Message("Saving of level {0} &Swas cancelled", lvl.ColoredName);
            return saved;
        }
        
        static void Save(Player p, Level lvl, string restoreName) {
            if (!TrySave(p, lvl, true)) return;
            p.Message("Level {0} &Ssaved", lvl.ColoredName);
            
            string backup = lvl.Backup(true, restoreName);
            if (backup == null) return;
            
            if (restoreName.Length == 0) {
                Logger.Log(LogType.SystemActivity, "Backup {1} saved for {0}", lvl.name, backup);
                lvl.Message("Backup " + backup + " saved for " + lvl.ColoredName);
            } else {
                Logger.Log(LogType.SystemActivity, "{0} had a backup created named &b{1}", lvl.name, backup);
                lvl.Message(lvl.ColoredName + " &Shad a backup created named &b" + backup);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Save &H- Saves the level you are currently in");
            p.Message("&T/Save all &H- Saves all loaded levels.");
            p.Message("&T/Save [level] &H- Saves the specified level.");
            p.Message("&T/Save [level] [name] &H- Backups the level with a given restore name");
        }
    }
}
