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
    public class CmdAscend : Command2 {
        public override string name { get { return "Ascend"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            if (!Hacks.CanUseHacks(p)) {
                p.Message("You cannot use &T/Ascend &Son this map."); return;
            }
            int x = p.Pos.FeetBlockCoords.X, y = p.Pos.FeetBlockCoords.Y, z = p.Pos.FeetBlockCoords.Z;
            if (y < 0) y = 0;
            
            int freeY = -1;
            if (p.level.IsValidPos(x, y, z)) {
                freeY = FindYAbove(p.level, (ushort)x, (ushort)y, (ushort)z);
            }
            
            if (freeY == -1) {
                p.Message("No free spaces found above you.");
            } else {
                p.Message("Teleported you up.");
                Position pos = Position.FromFeet(p.Pos.X, freeY * 32, p.Pos.Z);
                p.SendPos(Entities.SelfID, pos, p.Rot);
            }
        }
        
        static int FindYAbove(Level lvl, in ushort x, in ushort y, in ushort z) {
            bool foundAnySolid = false;
            for (ushort yCheck = y; yCheck < lvl.Height; yCheck++) {
                BlockID block = lvl.GetBlock(x, yCheck, z);
                if (block != Block.Invalid &&
                    CollideType.IsSolid(lvl.CollideType(block))) { foundAnySolid = true; continue; }
                    
                BlockID above = lvl.GetBlock(x, (ushort)(yCheck + 1), z);
                if (above != Block.Invalid &&
                    CollideType.IsSolid(lvl.CollideType(above))) { foundAnySolid = true; continue; }

                BlockID below = lvl.GetBlock(x, (ushort)(yCheck - 1), z);
                if (below != Block.Invalid &&
                    CollideType.IsSolid(lvl.CollideType(below)) && yCheck > y) return yCheck;
            }
            return foundAnySolid ? lvl.Height : -1;
        }
        
        public override void Help(Player p) {
            string name = Group.GetColoredName(LevelPermission.Operator);
            p.Message("&T/Ascend");
            p.Message("&HTeleports you to the first free space above you.");
            p.Message("&H  Cannot be used on maps which have -hax in their motd. " +
                           "(unless you are {0}&H+ and the motd has +ophax)", name);
        }
    }
}
