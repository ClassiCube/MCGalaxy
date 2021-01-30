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
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMode : Command2 {
        public override string name { get { return "Mode"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("TNT", "tnt") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            // Special handling for the old TNT command
            if (message.CaselessStarts("tnt ")) {
                string[] parts = message.SplitSpaces(2);
                if (parts[1].CaselessEq("small")) {
                    message = Block.GetName(p, Block.TNT_Small);
                } else if (parts[1].CaselessEq("big")) {
                    message = Block.GetName(p, Block.TNT_Big);
                } else if (parts[1].CaselessEq("nuke")) {
                    message = Block.GetName(p, Block.TNT_Nuke);
                }
            }
            
            if (message.Length == 0) {
                if (p.ModeBlock != Block.Invalid) {
                    p.Message("&b{0} &Smode: &cOFF", Block.GetName(p, p.ModeBlock));
                    p.ModeBlock = Block.Invalid;
                } else {
                    Help(p);
                }
                return;
            }
            
            BlockID block;
            if (!CommandParser.GetBlock(p, message, out block)) return;
            if (!CommandParser.IsBlockAllowed(p, "place", block)) return;
            
            if (p.ModeBlock == block) {
                p.Message("&b{0} &Smode: &cOFF", Block.GetName(p, p.ModeBlock));
                p.ModeBlock = Block.Invalid;
            } else {
                p.ModeBlock = block;
                p.Message("&b{0} &Smode: &aON", Block.GetName(p, p.ModeBlock));
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Mode");
            p.Message("&HReverts the last &T/Mode [block].");
            p.Message("&T/Mode [block]");
            p.Message("&HMakes every block placed into [block].");
            p.Message("&H/[block] also works");
            p.Message("&T/Mode tnt small/big/nuke &H");
            p.Message("&HMakes every block placed into exploding TNT (if physics on).");
        }
    }
}
