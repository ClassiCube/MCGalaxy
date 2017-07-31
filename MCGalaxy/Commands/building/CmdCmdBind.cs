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
    public sealed class CmdCmdBind : Command {        
        public override string name { get { return "CmdBind"; } }
        public override string shortcut { get { return "cb"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message.Length == 0) {
                bool anyBinds = false;
                for (int i = 0; i < p.CmdBindings.Length; i++)  {
                    if (p.CmdBindings[i] != null) {
                        Player.Message(p, "%T/{0} %Sbound to %T/{1} {2}", i, p.CmdBindings[i], p.CmdArgsBindings[i]);
                        anyBinds = true;
                    }
                }
                
                if (!anyBinds) Player.Message(p, "You currently have no commands bound.");
                return;
            }
            
            string[] parts = message.SplitSpaces(3);
            int j = 0;
            if (!CommandParser.GetInt(p, parts[0], "index", ref j, 0, p.CmdBindings.Length - 1)) return;
            
            if (parts.Length == 1) {
                if (p.CmdBindings[j] == null) {
                    Player.Message(p, "No command bound for %T/{0}", j);
                } else {
                    Player.Message(p, "%T/{0} %Sbound to %T/{1} {2}", j, p.CmdBindings[j], p.CmdArgsBindings[j]);
                }
            } else {
                p.CmdBindings[j] = parts[1];
                p.CmdArgsBindings[j] = parts.Length > 2 ? parts[2] : "";

                Player.Message(p, "Bound %T/{0} {1}%Sto %T/" + j, p.CmdBindings[j], p.CmdArgsBindings[j], j);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/CmdBind [num] [command]%H- Binds [num] to [command]");
            Player.Message(p, "%H  Use with \"%T/[num]%H\" &b(example: %T/2&b)");
            Player.Message(p, "%T/CmdBind [num] %H- Lists the command currently bound to [num]");
            Player.Message(p, "%T/CmdBind %H- Lists all bound commands");
            Player.Message(p, "%H[num] must be between 0 and 9");
        }
    }
}
