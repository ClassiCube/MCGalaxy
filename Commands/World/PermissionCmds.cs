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
using System.Collections.Generic;

namespace MCGalaxy.Commands.World {  
    public sealed class CmdPerbuildMax : PermissionCmd {        
        public override string name { get { return "perbuildmax"; } }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) { Help(p); return; }
            
            Group grp = null;
            Level lvl = GetArgs(p, args, ref grp);
            if (lvl != null) lvl.BuildAccess.SetMax(p, grp);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/perbuildmax [Level] [Rank]");
            Player.Message(p, "%HSets the highest rank able to build on the given level.");
        }
    }
    
    public sealed class CmdPermissionBuild : PermissionCmd {        
        public override string name { get { return "perbuild"; } }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) { Help(p); return; }
            
            Group grp = null;
            Level lvl = GetArgs(p, args, ref grp);
            if (lvl != null) lvl.BuildAccess.SetMin(p, grp);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/perbuild [Level] [Rank]");
            Player.Message(p, "%HSets the lowest rank able to build on the given level.");
        }
    }
    
    public sealed class CmdPervisitMax : PermissionCmd {        
        public override string name { get { return "pervisitmax"; } }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) { Help(p); return; }
            
            Group grp = null;
            Level lvl = GetArgs(p, args, ref grp);
            if (lvl != null) lvl.VisitAccess.SetMax(p, grp);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pervisitmax [level] [rank]");
            Player.Message(p, "%HSets the highest rank able to visit the given level.");
        }
    }
    
    public sealed class CmdPermissionVisit : PermissionCmd {       
        public override string name { get { return "pervisit"; } }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) { Help(p); return; }
            
            string name = args[args.Length - 1];
            if (name.Length > 0 && (name[0] == '+' || name[0] == '-')) {
                PermissionCmd.UseList(p, args, "pervisit", l => l.permissionvisit,
                                       l => l.VisitWhitelist, l => l.VisitBlacklist); return;
            }
            
            Group grp = null;
            Level lvl = GetArgs(p, args, ref grp);
            if (lvl != null) lvl.VisitAccess.SetMin(p, grp);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pervisit [level] [rank]");
            Player.Message(p, "%HSets the lowest rank able to visit the given level.");
            Player.Message(p, "%T/pervisit [level] +[name]");
            Player.Message(p, "%HAllows [name] to visit the map, even if their rank cannot.");
            Player.Message(p, "%T/pervisit [level] -[name]");
            Player.Message(p, "%HPrevents [name] from visiting the map, even if their rank can.");
        }
    }
}