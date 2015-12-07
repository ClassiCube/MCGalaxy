/*
    Copyright 2015 MCGalaxy
    
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
    
    static class CmdPermissions {
        
        public static void Use(Player p, string[] args, bool skipNobodyPerm, string target,
                               Func<Level, LevelPermission> getter, Action<Level, LevelPermission> setter) {
            if (args.Length == 1 && p == null) {
                Player.SendMessage(p, "You must provide a level name when running this command from console.");
                return;
            }
            
            Level level = args.Length == 1 ? p.level : Level.Find(args[0]);
            if (level == null) {
                Player.SendMessage(p, "There is no level \"" + args[1] + "\" loaded."); return;
            }
            string rank = args.Length == 1 ? args[0] : args[1];
            LevelPermission newRank = Level.PermissionFromName(rank);
            if (newRank == LevelPermission.Null) {
                Player.SendMessage(p, "Not a valid rank"); return;
            }

            if (p != null && getter(level) > p.group.Permission) {
                if (skipNobodyPerm || (getter(level) != LevelPermission.Nobody)) {
                    Player.SendMessage(p, "You cannot change the " + target + " of a level " +
                                       "with a " + target + " higher than your rank.");
                    return;
                }
            }
            
            setter(level, newRank);
            Level.SaveSettings(level);
            Server.s.Log(level.name + " " + target + " permission changed to " + newRank + ".");
            Player.GlobalMessageLevel(level, target + " permission changed to " + newRank + ".");
            if (p == null || p.level != level)
                Player.SendMessage(p, target + " permission changed to " + newRank + " on " + level.name + ".");
        }
    }
    
    public sealed class CmdPerbuildMax : Command {
        
        public override string name { get { return "perbuildmax"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPerbuildMax() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) {
                Help(p); return;
            }
            
            CmdPermissions.Use(
                p, args, false, "perbuildmax", l => l.perbuildmax,
                (l, v) => l.perbuildmax = v);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%a/perbuildmax [Level] [Rank]");
            Player.SendMessage(p, "%eSets the highest rank able to build on the given level.");
        }
    }
    
    public sealed class CmdPermissionBuild : Command {
        
        public override string name { get { return "perbuild"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPermissionBuild() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) {
                Help(p); return;
            }
            
            CmdPermissions.Use(
                p, args, true, "perbuild", l => l.permissionbuild,
                (l, v) => l.permissionbuild = v);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%a/perbuild [Level] [Rank]");
            Player.SendMessage(p, "%eSets the lowest rank able to build on the given level.");
        }
    }
    
    public sealed class CmdPervisitMax : Command {
        
        public override string name { get { return "pervisitmax"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPervisitMax() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) {
                Help(p); return;
            }
            
            CmdPermissions.Use(
                p, args, false, "pervisitmax", l => l.pervisitmax,
                (l, v) => l.pervisitmax = v);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%a/pervisitmax [level] [rank]");
            Player.SendMessage(p, "%eSets the highest rank able to visit the given level.");
        }
    }
    
    public sealed class CmdPermissionVisit : Command {
        
        public override string name { get { return "pervisit"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPermissionVisit() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) {
                Help(p); return;
            }
            
            CmdPermissions.Use(
                p, args, true, "pervisit", l => l.permissionvisit,
                (l, v) => l.permissionvisit = v);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%a/pervisit [level] [rank]");
            Player.SendMessage(p, "%eSets the lowest rank able to visit the given level.");
        }
    }
}