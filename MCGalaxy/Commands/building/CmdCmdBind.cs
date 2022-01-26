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
                foreach (var kvp in p.CmdBindings)
                {
                    p.Message("&T/{0} &Sbound to &T/{1}", kvp.Key, kvp.Value);
                    anyBinds = true;
                }
                
                if (!anyBinds) p.Message("You currently have no commands bound.");
                return;
            }

            string[] parts = message.SplitSpaces(2);
            string trigger = parts[0];

            if (parts.Length == 1) {
                string value;
                if (!p.CmdBindings.TryGetValue(trigger, out value)) {
                    p.Message("No command bound for &T/{0}", trigger);
                } else {
                    p.Message("&T/{0} &Sbound to &T/{1}", trigger, value);
                }
            } else {
                p.CmdBindings[trigger] = parts[1];
                p.Message("Bound &T/{1} &Sto &T/{0}", trigger, parts[1]);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/CmdBind [shortcut] [command]");
            p.Message("&HBinds [shortcut] to [command]");
            p.Message("&H  Use with \"&T/[shortcut]&H\" &f(example: &T/2&f)");
            p.Message("&T/CmdBind [shortcut]");
            p.Message("&HLists the command currently bound to [shortcut]");
            p.Message("&T/CmdBind &H");
            p.Message("&HLists all currently bound commands");
        }
    }
}
