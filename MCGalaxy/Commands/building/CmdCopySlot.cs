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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdCopySlot : Command {
        public override string name { get { return "CopySlot"; } }
        public override string shortcut { get { return "cs"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message.Length == 0) {
                int used = 0;
                for (int i = 0; i < p.CopySlots.Count; i++) {
                    if (p.CopySlots[i] == null) continue;
                    Player.Message(p, "  #{0}: {1}", i + 1, p.CopySlots[i].Summary);
                    used++;
                }
                
                Player.Message(p, "Using {0} of {1} slots, with slot #{2} selected.",
                               used, p.group.CopySlots, p.CurrentCopySlot + 1);
            } else {
                int i = 0;
                if (!CommandParser.GetInt(p, message, "Slot number", ref i, 1, p.group.CopySlots)) return;
                
                i--; p.CurrentCopySlot = i;
                if (i >= p.CopySlots.Count || p.CopySlots[i] == null) {
                    Player.Message(p, "Selected copy slot {0} (unused)", i + 1);
                } else {
                    Player.Message(p, "Selected copy slot {0}: {1}", i + 1, p.CopySlots[i].Summary);
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/CopySlot [number]");
            Player.Message(p, "%HSelects the slot to %T/copy %Hand %T/paste %Hfrom");
            Player.Message(p, "%HMaxmimum number of copy slots is determined your rank");
            Player.Message(p, "%T/CopySlot");
            Player.Message(p, "%HLists details about any copies stored in any slots");
        }
    }
}
