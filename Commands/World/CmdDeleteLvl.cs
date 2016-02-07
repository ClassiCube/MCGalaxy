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
using System.IO;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
    public sealed class CmdDeleteLvl : Command {
        
        public override string name { get { return "deletelvl"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdDeleteLvl() { }

        public override void Use(Player p, string message) {
            if (message == "" || message.Split(' ').Length > 1) { Help(p); return; }
            Level foundLevel = LevelInfo.Find(message);
            if (foundLevel != null) {
                if (p != null && foundLevel.permissionbuild > p.group.Permission) {
                    Player.SendMessage(p, "%cYou can't delete levels with a perbuild rank higher than yours!");
                    return;
                }
                foundLevel.Unload();
            }

            if (foundLevel == Server.mainLevel) { Player.SendMessage(p, "Cannot delete the main level."); return; }
            if (!Directory.Exists("levels/deleted")) Directory.CreateDirectory("levels/deleted");

            if (!File.Exists("levels/" + message + ".lvl")) {
                Player.SendMessage(p, "Could not find specified level."); return;
            }

            LevelPermission perbuild = GetPerBuildPermission(message);
            if (p != null && perbuild > p.group.Permission) {
                Player.SendMessage(p, "%cYou can't delete levels with a perbuild rank higher than yours!"); return;
            }

            if (File.Exists("levels/deleted/" + message + ".lvl")) {
                int currentNum = 0;
                while (File.Exists("levels/deleted/" + message + currentNum + ".lvl")) currentNum++;

                File.Move("levels/" + message + ".lvl", "levels/deleted/" + message + currentNum + ".lvl");
            } else {
                File.Move("levels/" + message + ".lvl", "levels/deleted/" + message + ".lvl");
            }
            Player.SendMessage(p, "Created backup.");

            try { File.Delete("levels/level properties/" + message + ".properties"); } catch { }
            try { File.Delete("levels/level properties/" + message); } catch { }
            try {
                if (File.Exists("blockdefs/lvl_" + message + ".json"))
                    File.Delete("blockdefs/lvl_" + message + ".json");
            } catch {}

            //safe against SQL injections because the levelname (message) is first being checked if it exists
            Database.executeQuery("DROP TABLE `Block" + message + "`");
            Database.executeQuery("DROP TABLE `Portals" + message + "`");
            Database.executeQuery("DROP TABLE `Messages" + message + "`");
            Database.executeQuery("DROP TABLE `Zone" + message + "`");
            Player.GlobalMessage("Level " + message + " was deleted.");
        }
        
        LevelPermission GetPerBuildPermission(string level) {
            string value = LevelInfo.FindOfflineProperty(level, "perbuild");
            if (value == null) return LevelPermission.Guest;
            Group grp = Group.Find(value);
            return grp == null ? LevelPermission.Guest : grp.Permission;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/deletelvl [map] - Completely deletes [map] (portals, MBs, everything");
            Player.SendMessage(p, "A backup of the map will be placed in the levels/deleted folder");
        }
    }
}
