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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdMissile : WeaponCmd {

        public override string name { get { return "missile"; } }
        protected override string Weapon { get { return "Missile"; } }

        protected override void PlacedMark(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            if (!p.staticCommands) {
                p.ClearBlockchange();
                p.aiming = false;
            }
            p.RevertBlock(x, y, z);
            if (!CommandParser.IsBlockAllowed(p, "place", block)) return;
            
            MissileArgs args = new MissileArgs();
            args.player = p;
            args.block = block;
            CatchPos bp = (CatchPos)p.blockchangeObject;
            args.ending = bp.ending;
            args.pos = MakePos(p);

            Thread gunThread = new Thread(() => DoShoot(args));
            gunThread.Name = "MCG_Missile";
            gunThread.Start();
        }
        
        class MissileArgs {
            public Player player;
            public ExtBlock block;
            public EndType ending;
            public Vec3U16 pos;
            
            public List<Vec3U16> previous = new List<Vec3U16>();
            public List<Vec3U16> allBlocks = new List<Vec3U16>();
            public List<Vec3S32> buffer = new List<Vec3S32>();
            public int iterations;
        }

        void DoShoot(MissileArgs args) {
            Player p = args.player;
            
            while (true) {
                args.iterations++;
                Vec3U16 target = MissileTarget(args);
                FindNext(target, ref args.pos, args.buffer);

                if (args.iterations <= 3) continue;
                if (!MoveMissile(args, args.pos, target)) break;
                Thread.Sleep(100);
            }

            if (args.ending == EndType.Teleport) {
                int index = args.previous.Count - 3;
                if (index >= 0 && index < args.previous.Count)
                    DoTeleport(p, args.previous[index]);
            }
            foreach (Vec3U16 pos1 in args.previous) {
                p.level.Blockchange(pos1.X, pos1.Y, pos1.Z, ExtBlock.Air, true);
                Thread.Sleep(100);
            }
        }
        
        static Vec3U16 MissileTarget(MissileArgs args) {
            Player p = args.player;
            Vec3U16 start = MakePos(p);
            Vec3F32 dir = DirUtils.GetFlatDirVector(p.Rot.RotY, p.Rot.HeadX);
            Vec3U16 target;
            int i;
            
            for (i = 1; ; i++) {
                target.X = (ushort)Math.Round(start.X + (double)(dir.X * i));
                target.Y = (ushort)Math.Round(start.Y + (double)(dir.Y * i));
                target.Z = (ushort)Math.Round(start.Z + (double)(dir.Z * i));

                ExtBlock block = p.level.GetBlock(target.X, target.Y, target.Z);
                if (block.BlockID == Block.Invalid) break;

                if (block.BlockID != Block.air && !args.allBlocks.Contains(target) && HandlesHitBlock(p, block, args.ending, target, false))
                    break;

                Player hit = GetPlayer(p, target, true);
                if (hit != null) return MakePos(hit);
            }

            target.X = (ushort)Math.Round(start.X + (double)(dir.X * (i - 1)));
            target.Y = (ushort)Math.Round(start.Y + (double)(dir.Y * (i - 1)));
            target.Z = (ushort)Math.Round(start.Z + (double)(dir.Z * (i - 1)));
            return target;
        }
        
        static bool MoveMissile(MissileArgs args, Vec3U16 pos, Vec3U16 target) {
            Player p = args.player;
            ExtBlock block = p.level.GetBlock(pos.X, pos.Y, pos.Z);
            if (block.BlockID != Block.air && !args.allBlocks.Contains(pos) && HandlesHitBlock(p, block, args.ending, pos, true))
                return false;

            p.level.Blockchange(pos.X, pos.Y, pos.Z, args.block);
            args.previous.Add(pos);
            args.allBlocks.Add(pos);

            Player hitP = GetPlayer(p, pos, true);
            if (hitP != null) {
                if (p.level.physics >= 3 && args.ending >= EndType.Explode) {
                    hitP.HandleDeath((ExtBlock)Block.stone, " was blown up by " + p.ColoredName, true);
                } else {
                    hitP.HandleDeath((ExtBlock)Block.stone, " was hit a missile from " + p.ColoredName);
                }
                return false;
            }

            if (pos == target && p.level.physics >= 3 && args.ending >= EndType.Explode) {
                p.level.MakeExplosion(target.X, target.Y, target.Z, 2);
                return false;
            }

            if (args.previous.Count > 12) {
                p.level.Blockchange(args.previous[0].X, args.previous[0].Y, args.previous[0].Z, ExtBlock.Air, true);
                args.previous.RemoveAt(0);
            }
            return true;
        }
        
        static Vec3U16 MakePos(Player p) {
            return (Vec3U16)p.Pos.BlockCoords;
        }
        
        void FindNext(Vec3U16 lookedAt, ref Vec3U16 pos, List<Vec3S32> buffer) {
            LineDrawOp.DrawLine(pos.X, pos.Y, pos.Z, 2, lookedAt.X, lookedAt.Y, lookedAt.Z, buffer);
            Vec3U16 end = (Vec3U16)buffer[buffer.Count - 1];
            pos.X = end.X; pos.Y = end.Y; pos.Z = end.Z;
            buffer.Clear();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/missile [at end]");
            Player.Message(p, "%HAllows you to fire missiles at people. Differs from /gun in that the missile is guided.");
            Player.Message(p, "%HAvailable [at end] types: %Sexplode, destroy, tp");
        }
    }
}
