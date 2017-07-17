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

namespace MCGalaxy.Commands.Misc {
    public class CmdAscend : Command {
        public override string name { get { return "ascend"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (!Hacks.CanUseHacks(p, p.level)) {
                Player.Message(p, "You cannot use /ascend on this map."); return;
            }
            int x = p.Pos.BlockX, y = p.Pos.BlockY, z = p.Pos.BlockZ;
            if (y < 0) y = 0;
            
            int freeY = -1;
            if (p.level.IsValidPos(x, y, z)) {
                freeY = FindYAbove(p.level, (ushort)x, (ushort)y, (ushort)z);
            }
            
            if (freeY == -1) {
                Player.Message(p, "No free spaces found above you.");
            } else {
                Player.Message(p, "Teleported you up.");
                Position pos = Position.FromFeet(p.Pos.X, freeY * 32, p.Pos.Z);
                p.SendPos(Entities.SelfID, pos, p.Rot);
            }
        }
        
        static int FindYAbove(Level lvl, ushort x, ushort y, ushort z) {
            for (; y < lvl.Height; y++) {
                ExtBlock block = lvl.GetBlock(x, y, z);
                if (!block.IsInvalid && lvl.CollideType(block) == CollideType.Solid) continue;
                ExtBlock above = lvl.GetBlock(x, (ushort)(y + 1), z);
                if (!above.IsInvalid && lvl.CollideType(above) == CollideType.Solid) continue;

                ExtBlock below = lvl.GetBlock(x, (ushort)(y - 1), z);
                if (!below.IsInvalid && lvl.CollideType(below) == CollideType.Solid)
                    return y;
            }
            return -1;
        }
        
        public override void Help(Player p) {
            string name = Group.GetColoredName(LevelPermission.Operator);
            Player.Message(p, "%T/ascend");
            Player.Message(p, "%HTeleports you to the first free space above you.");
            Player.Message(p, "%H  Cannot be used on maps which have -hax in their motd. " +
                           "(unless you are {0}%H+ and the motd has +ophax)", name);
        }
    }
}
