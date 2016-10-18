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
        
        public override void Help(Player p) { MaxHelp(p, "build on"); }
    }
    
    public sealed class CmdPermissionBuild : PermissionCmd {        
        public override string name { get { return "perbuild"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("wbuild"), new CommandAlias("worldbuild") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) { Help(p); return; }
            
            string name = args[args.Length - 1];
            if (name.Length > 0 && (name[0] == '+' || name[0] == '-')) {
                UseList(p, args, false); return;
            }
            
            Group grp = null;
            Level lvl = GetArgs(p, args, ref grp);
            if (lvl != null) lvl.BuildAccess.SetMin(p, grp);
        }
        
        public override void Help(Player p) { NormalHelp(p, "build on", "build"); }
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
        
        public override void Help(Player p) { MaxHelp(p, "visit"); }
    }
    
    public sealed class CmdPermissionVisit : PermissionCmd {       
        public override string name { get { return "pervisit"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("waccess"), new CommandAlias("worldaccess") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 2) { Help(p); return; }
            
            string name = args[args.Length - 1];
            if (name.Length > 0 && (name[0] == '+' || name[0] == '-')) {
                UseList(p, args, true); return;
            }
            
            Group grp = null;
            Level lvl = GetArgs(p, args, ref grp);
            if (lvl != null) lvl.VisitAccess.SetMin(p, grp);
        }
        
        public override void Help(Player p) { NormalHelp(p, "visit", "visit"); }
    }
}