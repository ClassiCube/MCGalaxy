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

namespace MCGalaxy.Commands {   
    public sealed class CmdCmdBind : Command {        
        public override string name { get { return "cmdbind"; } }
        public override string shortcut { get { return "cb"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdCmdBind() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }

            if (message == "") {
                bool foundBind = false;
                for (int i = 0; i < 10; i++)  {
                    if (p.cmdBind[i] != null) {
                        Player.Message(p, "&c/" + i + " %Sbound to &b" + p.cmdBind[i] + " " + p.messageBind[i]);
                        foundBind = true;
                    }
                }
                if (!foundBind) Player.Message(p, "You currently have no commands bound.");
                return;
            }
            
            string[] parts = message.Split(trimChars, 3);
            byte index;
            if (!byte.TryParse(parts[0], out index) || index >= 10) {
                Player.Message(p, "Bind number must be between 0 and 9."); return;
            }
            
            if (parts.Length == 1) {
                if (p.cmdBind[index] == null)
                    Player.Message(p, "No command bound for &c/" + index);
                else
                    Player.Message(p, "&c/" + index + " %Sbound to &b" + p.cmdBind[index] + " " + p.messageBind[index]);
            } else {
                p.cmdBind[index] = parts[1];
                p.messageBind[index] = parts.Length > 2 ? parts[2] : "";

                Player.Message(p, "Bound &b/" + p.cmdBind[index] + " " + p.messageBind[index] + " to &c/" + index);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/cmdbind [num] [command] - Binds [num] to [command]");
            Player.Message(p, "[num] must be between 0 and 9");
            Player.Message(p, "Use with \"/[num]\" &b(example: /2)");
            Player.Message(p, "Use /cmdbind [num] to see the currently bound command.");
            Player.Message(p, "Use /cmdbind to see all bound commands.");
        }
    }
}
