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
using System;
using System.Threading;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdChain : Command {
        public override string name { get { return "chain"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (!p.level.BuildAccess.CheckDetailed(p)) return;
            
            Level lvl = p.level;
            int x = p.Pos.BlockX, y = p.Pos.BlockY, z = p.Pos.BlockZ;
            if (x < 0 || z < 0 || x >= lvl.Width || z >= lvl.Length) {
                Player.Message(p, "You must be inside the map to use this command."); return;
            }
            
            int dirX = 0, dirZ = 0;
            DirUtils.EightYaw(p.Rot.RotY, out dirX, out dirZ);
            DoChain(p, (ushort)x, (ushort)y, (ushort)z, dirX, dirZ);
        }
        
        void DoChain(Player p, ushort x, ushort y, ushort z, int dirX, int dirZ) {
            Vec3U16 cur, next, target;
            cur.X = next.X = target.X = x;
            cur.Y = next.Y = target.Y = y;
            cur.Z = next.Z = target.Z = z;
            target.X = (ushort)(target.X + dirX);
            target.Z = (ushort)(target.Z + dirZ);
            
            for (int i = 0; ; i++) {
                cur.X = (ushort)(x + i * dirX);
                cur.Z = (ushort)(z + i * dirZ);
                next.X = (ushort)(cur.X + dirX);
                next.Z = (ushort)(cur.Z + dirZ);
                
                if (next.X >= p.level.Width || next.Z >= p.level.Length) {
                    if (i == 0) return;
                    PullBack(p, cur, target, dirX, dirZ);
                    p.level.Blockchange(p, x, y, z, ExtBlock.Air); return;
                }

                Thread.Sleep(250);
                p.level.Blockchange(p, cur.X, cur.Y, cur.Z, (ExtBlock)Block.mushroom);
                if (!p.level.IsAirAt(next.X, next.Y, next.Z)) {
                    PullBack(p, next, target, dirX, dirZ); 
                    p.level.Blockchange(p, x, y, z, ExtBlock.Air); return;
                }
            }
        }
        
        void PullBack(Player p, Vec3U16 cur, Vec3U16 target, int dirX, int dirZ) {
            ExtBlock block = p.level.GetBlock(cur.X, cur.Y, cur.Z);
            p.level.Blockchange(p, cur.X, cur.Y, cur.Z, block);
            
            while (cur.X != target.X || cur.Z != target.Z) {
                ExtBlock curBlock = p.level.GetBlock(cur.X, cur.Y, cur.Z);                
                if (curBlock == block) p.level.Blockchange(p, cur.X, cur.Y, cur.Z, ExtBlock.Air);

                cur.X = (ushort)(cur.X - dirX); cur.Z = (ushort)(cur.Z - dirZ);
                if (cur.X >= p.level.Width || cur.Z >= p.level.Length) return;
                
                curBlock = p.level.GetBlock(cur.X, cur.Y, cur.Z);
                if (curBlock.BlockID == Block.mushroom)
                    p.level.Blockchange(p, cur.X, cur.Y, cur.Z, block);
                Thread.Sleep(250);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/chain");
            Player.Message(p, "%HShoots a chain of brown mushrooms and grabs a block and brings it back to the start.");
        }
    }
}