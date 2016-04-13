/*
    Copyright 2012 MCGalaxy
    
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
    
    public sealed class CmdLevels : Command {
        
        public override string name { get { return "levels"; } }
        public override string shortcut { get { return "maps"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("worlds") }; }
        }
        public CmdLevels() { }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }

            string canVisit = "", canBuild = "";
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                if (p == null || lvl.permissionvisit <= p.group.Permission) {
                    if (Group.findPerm(lvl.permissionbuild) != null)
                        canVisit += ", " + Group.findPerm(lvl.permissionbuild).color + lvl.name + " &b[&f" + lvl.physics + "&b]";
                    else
                        canVisit += ", " + lvl.name + " &b[" + lvl.physics + "]";
                } else {
                    if (Group.findPerm(lvl.permissionvisit) != null)
                        canBuild += ", " + Group.findPerm(lvl.permissionvisit).color + lvl.name + " &b[&f" + lvl.physics + "&b]";
                    else
                        canBuild += ", " + lvl.name + " &b[&f" + lvl.physics + "&b]";
                }
            }

            if (canVisit != "")
                canVisit = canVisit.Remove(0, 2);
            Player.SendMessage(p, "Loaded levels [physics_level]: " + canVisit);
            if (canBuild != "")
                Player.SendMessage(p, "Loaded levsl you cannot visit: " + canBuild.Remove(0, 2));
            Player.SendMessage(p, "Use &f/unloaded" + Server.DefaultColor + " for unloaded levels.");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%f/levels " + Server.DefaultColor + "- Lists all loaded levels and their physics levels.");
        }
    }
}
