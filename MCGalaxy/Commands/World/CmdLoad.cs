/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;

namespace MCGalaxy.Commands.World {
    public sealed class CmdLoad : Command {
        public override string name { get { return "load"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("mapload"), new CommandAlias("wload") }; }
        }
        public CmdLoad() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            if (args.Length > 2) { Help(p); return; }

            string phys = args.Length > 1 ? args[1] : "0";
            LoadLevel(p, args[0], phys, false);
        }
        
        public static Level LoadLevel(Player p, string name,
                                      string phys = "0", bool autoLoaded = false) {
            name = name.ToLower();
            try {
                return LoadLevelCore(p, name, phys, autoLoaded);
            } finally {
                Server.DoGC();
            }
        }
        
        static Level LoadLevelCore(Player p, string name,
                                   string phys, bool autoLoaded) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level l in loaded) {
                if (l.name == name) { Player.Message(p, "Level {0} %Sis already loaded.", l.ColoredName); return null; }
            }
            if (!LevelInfo.MapExists(name)) {
                Player.Message(p, "Level \"{0}\" does not exist", name); return null;
            }
            
            Level lvl = ReadLevel(p, name);
            if (lvl == null || !lvl.CanJoin(p)) return null;

            loaded = LevelInfo.Loaded.Items;
            foreach (Level l in loaded) {
                if (l.name == name) { Player.Message(p, "Level {0} %Sis already loaded.", l.ColoredName); return null; }
            }

            LevelInfo.Loaded.Add(lvl);
            if (!autoLoaded)
                Chat.MessageWhere("Level {0} %Sloaded.", pl => Entities.CanSee(pl, p), lvl.ColoredName);
            
            int physLevel;
            if (!int.TryParse(phys, out physLevel)) {
                Player.Message(p, "Physics must be an integer between 0 and 5."); return lvl;
            }
            if (physLevel >= 1 && physLevel <= 5) lvl.setPhysics(physLevel);
            return lvl;
        }
        
        static Level ReadLevel(Player p, string name) {
            Level level = Level.Load(name);
            if (level != null) return level;
            if (!File.Exists(LevelInfo.MapPath(name) + ".backup")) {
                Player.Message(p, "Backup of {0} does not exist.", name); return null;
            }
            
            Server.s.Log("Attempting to load backup");
            level = Level.Load(name, LevelInfo.MapPath(name) + ".backup", 0);
            if (level != null) return level;
            
            Player.Message(p, "Loading backup failed.");
            string backupPath = Server.backupLocation;
            
            if (Directory.Exists(backupPath + "/" + name)) {
                int num = Directory.GetDirectories(backupPath + "/" + name).Length;
                Server.s.Log("Attempting to load latest backup, number " + num + " instead.");
                
                string path = LevelInfo.BackupPath(name, num.ToString());
                level = Level.Load(name, path, 0);
                if (level == null)
                    Player.Message(p, "Loading latest backup failed as well.");
            } else {
                Player.Message(p, "Latest backup of {0} does not exist.", name);
            }
            return level;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/load [level] <physics>");
            Player.Message(p, "%HLoads a level.");
        }
    }
}
