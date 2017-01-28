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
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            string[] args = message.ToLower().Split(' ');
            if (args.Length > 2) { Help(p); return; }
            
            if (args[0] == "clear") {
                for (byte id = 0; id < Block.CpeCount; id++) p.bindings[id] = id;
                Player.Message(p, "All bindings were unbound.");
                return;
            }

            if (args.Length == 2) {
                byte src = Block.Byte(args[0]);
                byte dst = Block.Byte(args[1]);
                if (src == Block.Invalid) { Player.Message(p, "There is no block \"{0}\".", args[0]); return; }
                if (dst == Block.Invalid) { Player.Message(p, "There is no block \"{0}\".", args[1]); return; }

                if (!Block.Placable(src)) { Player.Message(p, "{0} isn't a special block.", Block.Name(src)); return; }
                if (!Block.canPlace(p, dst)) { Formatter.MessageBlock(p, "bind ", dst); return; }
                if (src >= Block.CpeCount) { Player.Message(p, "Cannot bind anything to this block."); return; }
                
                p.bindings[src] = dst;
                Player.Message(p, "{0} bound to {1}", Block.Name(src), Block.Name(dst));
            } else {
                byte src = Block.Byte(args[0]);
                if (src >= Block.CpeCount) { Player.Message(p, "This block cannot be bound"); return; }

                if (p.bindings[src] == src) { Player.Message(p, "{0} is not bound.", Block.Name(src)); return; }
                p.bindings[src] = src; 
                Player.Message(p, "Unbound {0}.", Block.Name(src));
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
