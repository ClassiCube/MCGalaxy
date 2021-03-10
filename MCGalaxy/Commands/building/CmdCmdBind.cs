/*
    Copyright 2011 MCForge
    
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

namespace MCGalaxy.Commands.Building {   
    public sealed class CmdCmdBind : Command2 {        
        public override string name { get { return "CmdBind"; } }
        public override string shortcut { get { return "cb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                bool anyBinds = false;
                for (int i = 0; i < p.CmdBindings.Length; i++)  {
                    if (p.CmdBindings[i] != null) {
                        p.Message("&T/{0} &Sbound to &T/{1}", i, p.CmdBindings[i]);
                        anyBinds = true;
                    }
                }
                
                if (!anyBinds) p.Message("You currently have no commands bound.");
                return;
            }
            
            string[] parts = message.SplitSpaces(2);
            int j = 0;
            if (!CommandParser.GetInt(p, parts[0], "index", ref j, 0, p.CmdBindings.Length - 1)) return;
            
            if (parts.Length == 1) {
                if (p.CmdBindings[j] == null) {
                    p.Message("No command bound for &T/{0}", j);
                } else {
                    p.Message("&T/{0} &Sbound to &T/{1}", j, p.CmdBindings[j]);
                }
            } else {
                p.CmdBindings[j] = parts[1];
                p.Message("Bound &T/{1} &Sto &T/{0}", j, p.CmdBindings[j]);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/CmdBind [num] [command]&H- Binds [num] to [command]");
            p.Message("&H  Use with \"&T/[num]&H\" &b(example: &T/2&b)");
            p.Message("&T/CmdBind [num] &H- Lists the command currently bound to [num]");
            p.Message("&T/CmdBind &H- Lists all bound commands");
            p.Message("&H[num] must be between 0 and 9");
        }
    }
}
