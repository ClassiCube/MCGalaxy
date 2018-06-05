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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdMissile : WeaponCmd {

        public override string name { get { return "Missile"; } }
        protected override string Weapon { get { return "Missile"; } }

        protected override void PlacedMark(Player p, ushort x, ushort y, ushort z, BlockID block) {
            if (!p.staticCommands) {
                p.ClearBlockchange();
                p.aiming = false;
            }
            p.RevertBlock(x, y, z);
            if (!p.level.Config.Guns || !CommandParser.IsBlockAllowed(p, "place", block)) {
                p.ClearBlockchange(); return;
            }
            
            WeaponArgs args = new WeaponArgs();
            args.player = p;
            args.block = block;
            args.weaponType = (WeaponType)p.blockchangeObject;
            args.pos = MakePos(p);

            SchedulerTask task = new SchedulerTask(MissileCallback, args,
                                                   TimeSpan.FromMilliseconds(100), true);
            p.CriticalTasks.Add(task);
        }

        static void MissileCallback(SchedulerTask task) {
            WeaponArgs args = (WeaponArgs)task.State;
            if (args.moving) { PerformMove(args); return; }
            
            args.TeleportSourcePlayer();
            if (args.previous.Count > 0) {
                Vec3U16 pos = args.previous[0];
                args.previous.RemoveAt(0);
                args.player.level.Blockchange(pos.X, pos.Y, pos.Z, Block.Air, true);
            }
            task.Repeating = args.previous.Count > 0;
        }
        
        static void PerformMove(WeaponArgs args) {
            while (true) {
                args.iterations++;
                Vec3U16 target = MissileTarget(args);
                FindNext(target, ref args.pos, args.buffer);

                if (args.iterations <= 3) continue;
                args.moving = MoveMissile(args, args.pos, target);
                return;
            }
        }
        
        
        static Vec3U16 MissileTarget(WeaponArgs args) {
            Player p = args.player;
            args.start = MakePos(p);
            args.dir = DirUtils.GetFlatDirVector(p.Rot.RotY, p.Rot.HeadX);
            int i;
            
            for (i = 1; ; i++) {
                Vec3U16 target = args.PosAt(i);
                BlockID block = p.level.GetBlock(target.X, target.Y, target.Z);
                if (block == Block.Invalid) break;

                if (block != Block.Air && !args.allBlocks.Contains(target) && HandlesHitBlock(p, block, args.weaponType, target, false))
                    break;

                Player hit = GetPlayer(p, target, true);
                if (hit != null) return MakePos(hit);
            }
            return args.PosAt(i - 1);
        }
        
        static bool MoveMissile(WeaponArgs args, Vec3U16 pos, Vec3U16 target) {
            Player p = args.player;
            BlockID block = p.level.GetBlock(pos.X, pos.Y, pos.Z);
            if (block != Block.Air && !args.allBlocks.Contains(pos) && HandlesHitBlock(p, block, args.weaponType, pos, true))
                return false;

            p.level.Blockchange(pos.X, pos.Y, pos.Z, args.block);
            args.previous.Add(pos);
            args.allBlocks.Add(pos);
            if (HitsPlayer(args, pos)) return false;

            if (pos == target && p.level.physics >= 3 && args.weaponType >= WeaponType.Explode) {
                p.level.MakeExplosion(target.X, target.Y, target.Z, 2);
                return false;
            }

            if (args.previous.Count > 12) {
                pos = args.previous[0];
                p.level.Blockchange(pos.X, pos.Y, pos.Z, Block.Air, true);
                args.previous.RemoveAt(0);
            }
            return true;
        }       
        
        static bool HitsPlayer(WeaponArgs args, Vec3U16 pos) {
            Player pl = GetPlayer(args.player, pos, true);
            if (pl == null) return false;
            
            Player p = args.player;
            if (p.level.physics >= 3 && args.weaponType >= WeaponType.Explode) {
                pl.HandleDeath(Block.Cobblestone, "@p %Swas blown up by " + p.ColoredName, true);
            } else {
                pl.HandleDeath(Block.Cobblestone, "@p %Swas hit by a missile from " + p.ColoredName);
            }
            return true;
        }
        
        static Vec3U16 MakePos(Player p) { return (Vec3U16)p.Pos.BlockCoords; }
        
        static void FindNext(Vec3U16 lookedAt, ref Vec3U16 pos, List<Vec3S32> buffer) {
            LineDrawOp.DrawLine(pos.X, pos.Y, pos.Z, 2, lookedAt.X, lookedAt.Y, lookedAt.Z, buffer);
            Vec3U16 end = (Vec3U16)buffer[buffer.Count - 1];
            pos.X = end.X; pos.Y = end.Y; pos.Z = end.Z;
            buffer.Clear();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Missile [at end]");
            Player.Message(p, "%HAllows you to fire missiles at people. Differs from %T/gun %Hin that the missile is guided.");
            Player.Message(p, "%HAvailable [at end] types: %Sexplode, destroy, tp");
        }
    }
}
