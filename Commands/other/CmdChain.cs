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
namespace MCGalaxy.Commands
{
    public sealed class CmdChain : Command
    {
        public override string name { get { return "chain"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdChain() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (p.level.permissionbuild > p.Rank) {
                Player.Message(p, "You cannot build on this map!"); return;
            }
            
            Level lvl = p.level;
            ushort x = (ushort)(p.pos[0] / 32), y = (ushort)(p.pos[1] / 32), z = (ushort)(p.pos[2] / 32);
            if (x >= lvl.Width || z >= lvl.Length) {
                Player.Message(p, "You must be inside the map to use this command."); return;
            }
            
            int dirX = 0, dirZ = 0;
            DirUtils.EightYaw(p.rot[0], out dirX, out dirZ);
            DoChain(p, x, y, z, dirX, dirZ);       
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
                    p.level.Blockchange(p, x, y, z, 0); return;
                }

                Thread.Sleep(250);
                p.level.Blockchange(p, cur.X, cur.Y, cur.Z, Block.mushroom);
                if (p.level.GetTile(next.X, next.Y, next.Z) != 0) {
                    PullBack(p, next, target, dirX, dirZ); 
                    p.level.Blockchange(p, x, y, z, 0); return;
                }
            }
        }
        
        void PullBack(Player p, Vec3U16 cur, Vec3U16 target, int dirX, int dirZ) {
            byte type = p.level.GetTile(cur.X, cur.Y, cur.Z), extType = 0;
            if (type == Block.custom_block)
                extType = p.level.GetExtTile(cur.X, cur.Y, cur.Z);
            p.level.Blockchange(p, cur.X, cur.Y, cur.Z, type);
            
            while (cur.X != target.X || cur.Z != target.Z) {
                byte tile = p.level.GetTile(cur.X, cur.Y, cur.Z), extTile = 0;
                if (tile == Block.custom_block)
                    extTile = p.level.GetExtTile(cur.X, cur.Y, cur.Z);
                
                if (tile == type) {
                    if (tile != Block.custom_block || (tile == Block.custom_block && extTile == extType))
                        p.level.Blockchange(p, cur.X, cur.Y, cur.Z, 0);
                }

                cur.X = (ushort)(cur.X - dirX); cur.Z = (ushort)(cur.Z - dirZ);
                if (cur.X >= p.level.Width || cur.Z >= p.level.Length) return;
                
                tile = p.level.GetTile(cur.X, cur.Y, cur.Z);
                if (tile == Block.mushroom)
                    p.level.Blockchange(p, cur.X, cur.Y, cur.Z, type, extType);
                Thread.Sleep(250);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/chain");
            Player.Message(p, "%HShoots a chain of brown mushrooms and grabs a block and brings it back to the start.");
        }
    }
}