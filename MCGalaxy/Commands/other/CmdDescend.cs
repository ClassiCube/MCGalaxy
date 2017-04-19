/*
    Copyright 2011 MCForge
    
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
namespace MCGalaxy.Commands {
    public sealed class CmdDescend : Command {
        public override string name { get { return "descend"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdDescend() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (!Hacks.CanUseHacks(p, p.level)) {
                Player.Message(p, "You cannot use /descend on this map."); return;
            }
            if (p.Pos.Y < 51 + 4) { Player.Message(p, "No free spaces found below you."); return; }
            // Move starting position down half a block since players are a little bit above the ground.
            int x = p.Pos.BlockX, y = (p.Pos.Y - 51 - 4) / 32, z = p.Pos.BlockZ;
            
            if (y > p.level.Height) y = p.level.Height;
            y--; // start at block below initially
            
            for (; y > 0; y--) {
                byte block = p.level.GetBlock(x, y, z);
                if (!(Block.Convert(block) == Block.air || block == Block.Invalid)) continue;               
                byte above = p.level.GetBlock(x, y + 1, z);             
                if (!(Block.Convert(above) == Block.air || above == Block.Invalid)) continue;
                
                byte below = p.level.GetBlock(x, y - 1, z);
                if (Solid(Block.Convert(below))) {
                    Player.Message(p, "Teleported you down.");
                    
                    Position pos = Position.FromFeet(p.Pos.X, y * 32, p.Pos.Z);
                    p.SendPos(Entities.SelfID, pos, p.Rot);
                    return;
                }
            }
            Player.Message(p, "No free spaces found below you.");
        }
        
        static bool Solid(byte b) {
            return b != Block.air && (b < Block.water || b > Block.lavastill) && b != Block.Invalid
                && b != Block.shrub && (b < Block.yellowflower || b > Block.redmushroom);
        }
        
        public override void Help(Player p) {
            string name = Group.GetColoredName(LevelPermission.Operator);
            Player.Message(p, "%T/descend");
            Player.Message(p, "%HTeleports you to the first free space below you.");
            Player.Message(p, "%H  Does not work on maps which have -hax in their motd. " +
                           "(unless you are {0}%H+ and the motd also has +ophax)", name);
        }
    }
}
