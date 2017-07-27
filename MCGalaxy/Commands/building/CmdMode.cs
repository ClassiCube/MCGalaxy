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
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("tnt", "tnt") }; }
        }

        public override void Use(Player p, string message) {
            // Special handling for the old TNT command
            if (message.CaselessStarts("tnt ")) {
                string[] parts = message.SplitSpaces(2);
                if (parts[1].CaselessEq("small")) {
                    message = Block.Name(Block.TNT_Small);
                } else if (parts[1].CaselessEq("big")) {
                    message = Block.Name(Block.TNT_Big);
                } else if (parts[1].CaselessEq("nuke")) {
                    message = Block.Name(Block.TNT_Nuke);
                }
            }
            
            if (message == "") {
                if (p.ModeBlock != ExtBlock.Air) {
                    Player.Message(p, "&b{0} %Smode: &cOFF", p.level.BlockName(p.ModeBlock));
                    p.ModeBlock = ExtBlock.Air;
                } else {
                    Help(p);
                }
                return;
            }
            
            ExtBlock block;
            if (!CommandParser.GetBlock(p, message, out block)) return;
            if (block == ExtBlock.Air) { Player.Message(p, "Cannot use Air Mode."); return; }
            if (!CommandParser.IsBlockAllowed(p, "place", block)) return;
            
            if (p.ModeBlock == block) {
                Player.Message(p, "&b{0} %Smode: &cOFF", p.level.BlockName(p.ModeBlock));
                p.ModeBlock = ExtBlock.Air;
            } else {
                p.ModeBlock = block;
                Player.Message(p, "&b{0} %Smode: &aON", p.level.BlockName(p.ModeBlock));
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mode");
            Player.Message(p, "%HReverts the last %T/mode [block].");
            Player.Message(p, "%T/mode [block]");
            Player.Message(p, "%HMakes every block placed into [block].");
            Player.Message(p, "%H/[block] also works");
            Player.Message(p, "%T/mode tnt small/big/nuke %H");
            Player.Message(p, "%HMakes every block placed into exploding TNT (if physics on).");
        }
    }
}
