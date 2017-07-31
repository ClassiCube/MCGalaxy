/*
    Written by Jack1312

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
using System.IO;

namespace MCGalaxy.Commands.World {   
    public class CmdCopyLVL : Command {        
        public override string name { get { return "CopyLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WCopy"), new CommandAlias("WorldCopy") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.ToLower().SplitSpaces();
            if (args.Length < 2) {
                Player.Message(p, "You did not specify the destination level name."); return;
            }
            
            string src = args[0], dst = args[1];
            if (p != null && !p.group.CanExecute("newlvl")) {
                Player.Message(p, "You cannot use /copylvl as you cannot use /newlvl."); return;
            }
            src = Matcher.FindMaps(p, src);
            if (src == null) return;
            if (!Formatter.ValidName(p, dst, "level")) return;
            if (LevelInfo.MapExists(dst)) { Player.Message(p, "The level \"" + dst + "\" already exists."); return; }

            try {
                LevelActions.CopyLevel(src, dst);
            } catch (System.IO.IOException) {
                Player.Message(p, "The level &c" + dst + " %Salready exists!"); return;
            }
            Player.Message(p, "The level &a" + src + " %Shas been copied to &a" + dst + ".");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/CopyLvl [level] [copied level]");
            Player.Message(p, "%HMakes a copy of [level] called [copied Level].");
            Player.Message(p, "%HNote: The level's BlockDB is not copied.");
        }
    }
}
