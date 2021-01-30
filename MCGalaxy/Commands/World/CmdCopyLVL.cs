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

namespace MCGalaxy.Commands.World {   
    public class CmdCopyLVL : Command2 {        
        public override string name { get { return "CopyLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WCopy"), new CommandAlias("WorldCopy") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.ToLower().SplitSpaces();
            if (args.Length < 2) {
                p.Message("You did not specify the destination level name."); return;
            }
            LevelConfig cfg;
            
            string src = Matcher.FindMaps(p, args[0]);
            if (src == null) return;
            if (!LevelInfo.Check(p, data.Rank, src, "copy this map", out cfg)) return;
            
            string dst = args[1];
            if (!Formatter.ValidMapName(p, dst)) return;

            if (!LevelActions.Copy(p, src, dst)) return;
            Chat.MessageGlobal("Level {0} &Swas copied to {1}", cfg.Color + src, cfg.Color + dst);
        }
        
        public override void Help(Player p) {
            p.Message("&T/CopyLvl [level] [copied level]");
            p.Message("&HMakes a copy of [level] called [copied level].");
            p.Message("&HNote: The level's BlockDB is not copied.");
        }
    }
}
