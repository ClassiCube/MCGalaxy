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
namespace MCGalaxy.Commands.Building {    
    public sealed class CmdBind : Command {
        public override string name { get { return "bind"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.ToLower().SplitSpaces();
            if (args.Length > 2) { Help(p); return; }
            
            if (args[0] == "clear") {
                for (int i = 0; i < p.BlockBindings.Length; i++) {
                    p.BlockBindings[i] = ExtBlock.FromRaw((byte)i);
                }
                Player.Message(p, "All bindings were unbound.");
                return;
            }

            if (args.Length == 2) {
                ExtBlock src, dst;
                if (!CommandParser.GetBlock(p, args[0], out src)) return;
                if (!CommandParser.GetBlock(p, args[1], out dst)) return;                
                if (!CommandParser.IsBlockAllowed(p, "bind a block to ", dst)) return;              
                if (src.IsPhysicsType) { 
                    Player.Message(p, "Physics blocks cannot be bound to another block."); return; 
                }
                
                p.BlockBindings[src.RawID] = dst;
                Player.Message(p, "{0} bound to {1}", p.level.BlockName(src), p.level.BlockName(dst));
            } else {
                ExtBlock src;
                if (!CommandParser.GetBlock(p, args[0], out src)) return;
                if (src.IsPhysicsType) { 
                    Player.Message(p, "Physics blocks cannot be bound to another block."); return; 
                }

                if (p.BlockBindings[src.RawID] == src) { 
                    Player.Message(p, "{0} is not bound.", p.level.BlockName(src)); return;
                }                
                p.BlockBindings[src.BlockID] = src; 
                Player.Message(p, "Unbound {0}.", p.level.BlockName(src));
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/bind [block] [replacement block]");
            Player.Message(p, "%HCauses [replacement] to be placed, whenever you place [block].");
            Player.Message(p, "%T/bind [block] %H- Removes binding for [block].");
            Player.Message(p, "%T/bind clear %H- Clears all binds.");
        }
    }
}
