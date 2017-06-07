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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Fun {  
    public sealed class CmdGun : WeaponCmd {
        public override string name { get { return "gun"; } }
        protected override string Weapon { get { return "Gun"; } }
        
        protected override void PlacedMark(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            p.RevertBlock(x, y, z);
            if (!CommandParser.IsBlockAllowed(p, "place", block)) return;

            Thread gunThread = new Thread(() => DoShoot(p, block));
            gunThread.Name = "MCG_Gun";
            gunThread.Start();
        }
        
        void DoShoot(Player p, ExtBlock block) {
            CatchPos bp = (CatchPos)p.blockchangeObject;
            Vec3F32 dir = DirUtils.GetFlatDirVector(p.Rot.RotY, p.Rot.HeadX);

            double bigDiag = Math.Sqrt(Math.Sqrt(p.level.Width * p.level.Width + p.level.Length * p.level.Length) 
                                       + p.level.Height * p.level.Height + p.level.Width * p.level.Width);

            List<Vec3U16> previous = new List<Vec3U16>();
            List<Vec3U16> allBlocks = new List<Vec3U16>();
            Vec3U16 pos;
            
            Vec3S32 start = p.Pos.BlockCoords;
            pos.X = (ushort)Math.Round(start.X + dir.X * 3);
            pos.Y = (ushort)Math.Round(start.Y + dir.Y * 3);
            pos.Z = (ushort)Math.Round(start.Z + dir.Z * 3);

            for (double t = 4; bigDiag > t; t++) {
                pos.X = (ushort)Math.Round(start.X + (double)(dir.X * t));
                pos.Y = (ushort)Math.Round(start.Y + (double)(dir.Y * t));
                pos.Z = (ushort)Math.Round(start.Z + (double)(dir.Z * t));

                byte by = p.level.GetTile(pos.X, pos.Y, pos.Z);
                if (by != Block.air && !allBlocks.Contains(pos) && HandlesHitBlock(p, by, bp.ending, pos, true))
                    break;

                p.level.Blockchange(pos.X, pos.Y, pos.Z, block);
                previous.Add(pos);
                allBlocks.Add(pos);

                if (HandlesPlayers(p, bp, pos)) break;

                if (t > 12 && bp.ending != EndType.Laser) {
                    pos = previous[0];
                    p.level.Blockchange(pos.X, pos.Y, pos.Z, ExtBlock.Air, true);
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

            foreach (Vec3U16 pos1 in previous) {
                p.level.Blockchange(pos1.X, pos1.Y, pos1.Z, ExtBlock.Air, true);
                if (bp.ending != EndType.Laser) Thread.Sleep(20);
            }
        }
        
        bool HandlesPlayers(Player p, CatchPos bp, Vec3U16 pos) {
            Player pl = GetPlayer(p, pos, true);
            if (pl == null) return false;
            
            ExtBlock stone = (ExtBlock)Block.stone;
            if (p.level.physics >= 3 && bp.ending >= EndType.Explode) {
                pl.HandleDeath(stone, " was blown up by " + p.ColoredName, true);
            } else {
                pl.HandleDeath(stone, " was shot by " + p.ColoredName);
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/gun [at end]");
            Player.Message(p, "%HAllows you to fire bullets at people");
            Player.Message(p, "%HAvailable [at end] types: %Sexplode, destroy, laser, tp");
        }
    }
}
