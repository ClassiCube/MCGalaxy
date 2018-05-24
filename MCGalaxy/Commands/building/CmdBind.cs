/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {    
    public sealed class CmdBind : Command {
        public override string name { get { return "Bind"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();
            if (args.Length > 2) { Help(p); return; }
            
            if (args[0].CaselessEq("clear")) {
                for (int b = 0; b < p.BlockBindings.Length; b++) {
                    p.BlockBindings[b] = (BlockID)b;
                }
                Player.Message(p, "All bindings were unbound.");
                return;
            }

            if (args.Length == 2) {
                BlockID src, dst;
                if (!CommandParser.GetBlock(p, args[0], out src)) return;
                if (!CommandParser.GetBlock(p, args[1], out dst)) return;
                if (!CommandParser.IsBlockAllowed(p, "bind a block to", dst)) return;              
                if (Block.IsPhysicsType(src)) {
                    Player.Message(p, "Physics blocks cannot be bound to another block."); return; 
                }
                
                p.BlockBindings[src] = dst;
                Player.Message(p, "{0} bound to {1}", Block.GetName(p, src), Block.GetName(p, dst));
            } else {
                BlockID src;
                if (!CommandParser.GetBlock(p, args[0], out src)) return;
                if (Block.IsPhysicsType(src)) {
                    Player.Message(p, "Physics blocks cannot be bound to another block."); return; 
                }

                if (p.BlockBindings[src] == src) { 
                    Player.Message(p, "{0} is not bound.", Block.GetName(p, src)); return;
                }                
                p.BlockBindings[src] = src; 
                Player.Message(p, "Unbound {0}.", Block.GetName(p, src));
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Bind [block] [replacement block]");
            Player.Message(p, "%HCauses [replacement] to be placed, whenever you place [block].");
            Player.Message(p, "%T/Bind [block] %H- Removes binding for [block].");
            Player.Message(p, "%T/Bind clear %H- Clears all binds.");
        }
    }
}
