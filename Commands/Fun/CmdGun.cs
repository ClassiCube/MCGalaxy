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
using System.Collections.Generic;
using System.Threading;

namespace MCGalaxy.Commands {
    
    public sealed class CmdGun : WeaponCmd
    {
        public override string name { get { return "gun"; } }
        public override string shortcut { get { return ""; } }
        protected override string Weapon { get { return "Gun"; } }
        
        protected override void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            p.RevertBlock(x, y, z);
            if (!Block.canPlace(p, type)) { Player.SendMessage(p, "You cannot place this block."); return; }

            Thread gunThread = new Thread(() => DoShoot(p, type, extType));
            gunThread.Name = "MCG_Gun";
            gunThread.Start();
        }
        
        void DoShoot(Player p, byte type, byte extType) {
            CatchPos bp = (CatchPos)p.blockchangeObject;
            double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
            double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
            double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

            double bigDiag = Math.Sqrt(Math.Sqrt(p.level.Width * p.level.Width + p.level.Length * p.level.Length) 
                                       + p.level.Height * p.level.Height + p.level.Width * p.level.Width);

            List<Pos> previous = new List<Pos>();
            List<Pos> allBlocks = new List<Pos>();
            Pos pos;
            
            ushort startX = (ushort)(p.pos[0] / 32);
            ushort startY = (ushort)(p.pos[1] / 32);
            ushort startZ = (ushort)(p.pos[2] / 32);

            pos.x = (ushort)Math.Round(startX + (double)(a * 3));
            pos.y = (ushort)Math.Round(startY + (double)(c * 3));
            pos.z = (ushort)Math.Round(startZ + (double)(b * 3));

            for (double t = 4; bigDiag > t; t++) {
                pos.x = (ushort)Math.Round(startX + (double)(a * t));
                pos.y = (ushort)Math.Round(startY + (double)(c * t));
                pos.z = (ushort)Math.Round(startZ + (double)(b * t));

                byte by = p.level.GetTile(pos.x, pos.y, pos.z);
                if (by != Block.air && !allBlocks.Contains(pos) && HandlesHitBlock(p, by, bp, pos, true))
                    break;

                p.level.Blockchange(pos.x, pos.y, pos.z, type, extType);
                previous.Add(pos);
                allBlocks.Add(pos);

                if (HandlesPlayers(p, bp, pos)) break;

                if (t > 12 && bp.ending != EndType.Laser) {
                    pos = previous[0];
                    p.level.Blockchange(pos.x, pos.y, pos.z, Block.air, true);
                    previous.RemoveAt(0);
                }
                
                if (bp.ending != EndType.Laser) Thread.Sleep(20);
            }

            if (bp.ending == EndType.Teleport) {
                int index = previous.Count - 3;
                if (index >= 0 && index < previous.Count)
                    DoTeleport(p, previous[index]);
            }
            if (bp.ending == EndType.Laser) Thread.Sleep(400);

            foreach (Pos pos1 in previous) {
                p.level.Blockchange(pos1.x, pos1.y, pos1.z, Block.air, true);
                if (bp.ending != EndType.Laser) Thread.Sleep(20);
            }
        }
        
        bool HandlesPlayers(Player p, CatchPos bp, Pos pos) {
            Player pl = GetPlayer(p, pos, true);
            if (pl == null) return false;
            
            if (p.level.physics >= 3 && bp.ending >= EndType.Explode)
                pl.HandleDeath(Block.stone, " was blown up by " + p.color + p.DisplayName, true);
            else
                pl.HandleDeath(Block.stone, " was shot by " + p.color + p.DisplayName);
            return true;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/gun [at end] - Allows you to fire bullets at people");
            Player.SendMessage(p, "Available [at end] values: &cexplode, destroy, laser, tp");
        }
    }
}
