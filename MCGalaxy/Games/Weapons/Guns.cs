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
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {

    /// <summary> Represents a gun weapon which dies when it hits a block or a player. </summary>
    /// <remarks> Fires in a straight line from where playing is looking. </remarks>
    public class Gun : Weapon {        
        public override string Name { get { return "Gun"; } }

        protected override void OnActivated(byte yaw, byte pitch, BlockID block) {
            AmmunitionData args = new AmmunitionData();
            args.block  = block;
            
            args.start = (Vec3U16)p.Pos.BlockCoords;
            args.dir = DirUtils.GetDirVector(yaw, pitch);
            args.pos = args.PosAt(3);
            args.iterations = 4;

            SchedulerTask task = new SchedulerTask(GunCallback, args, TimeSpan.Zero, true);
            p.CriticalTasks.Add(task);
        }
        
        protected virtual bool OnHitBlock(AmmunitionData args, Vec3U16 pos, BlockID block) {
            return true;
        }
        
        protected virtual void OnHitPlayer(AmmunitionData args, Player pl) {
            pl.HandleDeath(Block.Cobblestone, "@p %Swas shot by " + p.ColoredName);
        }
        
        protected virtual bool TickMove(AmmunitionData args) {
            if (args.iterations > 12) {
                Vec3U16 pos = args.visible[0];
                args.visible.RemoveAt(0);
                p.level.Blockchange(pos.X, pos.Y, pos.Z, Block.Air, true);
            }
            return true;
        }
        
        protected virtual bool TickRevert(SchedulerTask task) {
            AmmunitionData args = (AmmunitionData)task.State;
            
            if (args.visible.Count > 0) {
                Vec3U16 pos = args.visible[0];
                args.visible.RemoveAt(0);
                p.level.Blockchange(pos.X, pos.Y, pos.Z, Block.Air, true);
            }
            return args.visible.Count > 0;
        }
        
        void GunCallback(SchedulerTask task) {
            AmmunitionData args = (AmmunitionData)task.State;
            if (args.moving) {
                args.moving    = TickGun(args);
            } else {
                task.Repeating = TickRevert(task);
            }
        }
        
        bool TickGun(AmmunitionData args) {
            while (true) {
                args.pos = args.PosAt(args.iterations);
                args.iterations++;
                Vec3U16 pos = args.pos;

                BlockID cur = p.level.GetBlock(pos.X, pos.Y, pos.Z);
                if (cur == Block.Invalid) return false;
                if (cur != Block.Air && !args.all.Contains(pos) && OnHitBlock(args, pos, cur))
                    return false;

                p.level.Blockchange(pos.X, pos.Y, pos.Z, args.block);
                args.visible.Add(pos);
                args.all.Add(pos);
                
                Player pl = PlayerAt(p, pos, true);
                if (pl != null) { OnHitPlayer(args, pl); return false; }
                if (TickMove(args)) return true;
            }
        }
    }
    
    public class PenetrativeGun : Gun {
        public override string Name { get { return "Penetrative gun"; } }
        
        protected override bool OnHitBlock(AmmunitionData args, Vec3U16 pos, BlockID block) {
            if (p.level.physics < 2 || block == Block.Glass) return true;           
            // Penetrative gun goes through blocks lava can go through
            return !p.level.Props[block].LavaKills;
        }
    }
    
    public class ExplosiveGun : Gun {
        public override string Name { get { return "Explosive gun"; } }
        
        protected override bool OnHitBlock(AmmunitionData args, Vec3U16 pos, BlockID block) {
            if (p.level.physics >= 3 && block != Block.Glass) {
                p.level.MakeExplosion(pos.X, pos.Y, pos.Z, 1);
            }
            return true;
        }
        
        protected override void OnHitPlayer(AmmunitionData args, Player pl) {
            if (pl.level.physics >= 3) {
                pl.HandleDeath(Block.Cobblestone, "@p %Swas blown up by " + p.ColoredName, true);
            } else {
                base.OnHitPlayer(args, pl);
            }
        }
    }
    
    public class LaserGun : ExplosiveGun {
        public override string Name { get { return "Laser"; } }
        
        protected override bool TickMove(AmmunitionData args) {
            // laser immediately strikes target
            return false;
        }
        
        protected override bool TickRevert(SchedulerTask task) {
            AmmunitionData args = (AmmunitionData)task.State;
            
            if (args.all.Count > 0) {
                // laser persists for a short while
                task.Delay = TimeSpan.FromMilliseconds(400);
                args.all.Clear();
            } else {
                foreach (Vec3U16 pos in args.visible) {
                    p.level.Blockchange(pos.X, pos.Y, pos.Z, Block.Air, true);
                }
                args.visible.Clear();
            }
            return args.visible.Count > 0;
        }
    }
    
    public class TeleportGun : Gun {
        public override string Name { get { return "Teleporter gun"; } }
        
        void DoTeleport(AmmunitionData args) {
            int i = args.visible.Count - 3;
            if (i >= 0 && i < args.visible.Count) {
                Vec3U16 coords = args.visible[i];
                Position pos = new Position(coords.X * 32, coords.Y * 32 + 32, coords.Z * 32);
                p.SendPos(Entities.SelfID, pos, p.Rot);
            }
        }
        
        protected override void OnHitPlayer(AmmunitionData args, Player pl) {
            DoTeleport(args);
        }
        
        protected override bool OnHitBlock(AmmunitionData args, Vec3U16 pos, BlockID block) {
            DoTeleport(args);
            return true;
        }
    }
}