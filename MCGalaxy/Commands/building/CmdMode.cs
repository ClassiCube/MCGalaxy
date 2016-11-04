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
using System.Text;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMode : Command {
        public override string name { get { return "mode"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMode() { }

        public override void Use(Player p, string message) {
            if (message == "") {
                if (p.modeType != 0) {
                    Player.Message(p, "&b{0} %Smode: &cOFF", Block.Name(p.modeType).Capitalize());
                    p.modeType = 0;
                } else {
                    Help(p);
                }
                return;
            }
            
            byte block = Block.Byte(message);
            if (block == Block.Invalid) { Player.Message(p, "Could not find block given."); return; }
            if (block == Block.air) { Player.Message(p, "Cannot use Air Mode."); return; }

            if (!p.allowTnt && (block == Block.tnt || block == Block.bigtnt || block == Block.smalltnt
                                || block == Block.nuketnt || block == Block.tntexplosion)) {
                Player.Message(p, "Tnt usage is not allowed at the moment"); return;
            }
            if (!p.allowTnt && block == Block.fire) {
                Player.Message(p, "Tnt usage is not allowed at the moment, fire is a lighter for tnt and is also disabled"); return;
            }          
            if (!Block.canPlace(p, block)) { Formatter.MessageBlock(p, "place ", block); return; }
            
            string name = Block.Name(block).Capitalize();
            if (p.modeType == block) {
                Player.Message(p, "&b{0} %Smode: &cOFF", name);
                p.modeType = 0;
            } else {
                p.modeType = block;
                Player.Message(p, "&b{0} %Smode: &aON", name);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mode [block]");
            Player.Message(p, "%HMakes every block placed into [block].");
            Player.Message(p, "%H/[block] also works");
        }
    }
}
