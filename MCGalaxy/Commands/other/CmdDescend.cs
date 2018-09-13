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
using System;
using MCGalaxy.Blocks;
using MCGalaxy.Commands.Fun;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdDescend : Command2 {
        public override string name { get { return "Descend"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            if (!Hacks.CanUseHacks(p, p.level)) {
                p.Message("You cannot use %T/Descend %Son this map."); return;
            }

            // Move starting position down half a block since players are a little bit above the ground.
            int x = p.Pos.BlockX, y = (p.Pos.Y - 51 - 4) / 32, z = p.Pos.BlockZ;            
            if (y > p.level.Height) y = p.level.Height;
            y--; // start at block below initially
            
            int freeY = -1;
            if (p.level.IsValidPos(x, y, z)) {
                freeY = FindYBelow(p.level, (ushort)x, y, (ushort)z);
            }
            
            if (freeY == -1) {
                p.Message("No free spaces found below you.");
            } else {
                p.Message("Teleported you down.");
                Position pos = Position.FromFeet(p.Pos.X, freeY * 32, p.Pos.Z);
                p.SendPos(Entities.SelfID, pos, p.Rot);
            }
        }
        
        static int FindYBelow(Level lvl, ushort x, int y, ushort z) {
            for (; y >= 0; y--) {
                if (SolidAt(lvl, x, y    , z)) continue;
                if (SolidAt(lvl, x, y + 1, z)) continue;
                if (SolidAt(lvl, x, y - 1, z)) return y;
            }
            return -1;
        }
        
        static bool SolidAt(Level lvl, ushort x, int y, ushort z) {
            if (y >= lvl.Height) return false;
            BlockID block = lvl.GetBlock(x, (ushort)y, z);
            return CollideType.IsSolid(lvl.CollideType(block));
        }
        
        public override void Help(Player p) {
            string name = Group.GetColoredName(LevelPermission.Operator);
            p.Message("%T/Descend");
            p.Message("%HTeleports you to the first free space below you.");
            p.Message("%H  Cannot be used on maps which have -hax in their motd. " +
                           "(unless you are {0}%H+ and the motd has +ophax)", name);
        }
    }
}
