/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.BlockPhysics;

namespace MCGalaxy.Commands
{
    public sealed class CmdC4 : Command
    {
        public override string name { get { return "c4"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdC4() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            
            if (p.level.physics >= 1 && p.level.physics < 5) {
                sbyte numb = C4Physics.NextCircuit(p.level);
                C4Data c4 = new C4Data(numb);
                p.level.C4list.Add(c4);
                p.c4circuitNumber = numb;
                Player.SendMessage(p, "Place any block for c4 and place a &cred %Sblock for the detonator!");
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
            } else {
                Player.SendMessage(p, "To use c4, the physics level must be 1, 2, 3 or 4");
            }
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            p.ClearBlockchange();
            if (type == Block.red) {
                p.ManualChange(x, y, z, 1, Block.c4det, 0);
                Player.SendMessage(p, "Placed detonator block!");
                return;
            } else if (type != Block.air) {
                p.ManualChange(x, y, z, 1, Block.c4, 0);
            }
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/c4 - Place c4!");
        }
    }
}
