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
using MCGalaxy.Tasks;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdGun : WeaponCmd {
        public override string name { get { return "Gun"; } }
        protected override string Weapon { get { return "Gun"; } }
        
        protected override void PlacedMark(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            p.RevertBlock(x, y, z);
            if (!p.level.Config.Guns || !CommandParser.IsBlockAllowed(p, "place", block)) {
                p.ClearBlockchange(); return;
            }

            WeaponArgs args = new WeaponArgs();
            args.player = p;
            args.block = block;
            args.weaponType = (WeaponType)p.blockchangeObject;
            
            args.start = MakePos(p);
            args.dir = DirUtils.GetFlatDirVector(p.Rot.RotY, p.Rot.HeadX);
            args.pos = args.PosAt(3);
            args.iterations = 4;

            SchedulerTask task = new SchedulerTask(GunCallback, args, TimeSpan.Zero, true);
            p.CriticalTasks.Add(task);
        }
        
        static void GunCallback(SchedulerTask task) {
            WeaponArgs args = (WeaponArgs)task.State;
            if (args.moving) {
                args.moving = MoveGun(args);
                
                // Laser gun persists for a short while
                if (!args.moving && args.weaponType == WeaponType.Laser)
                    task.Delay = TimeSpan.FromMilliseconds(400);
                return;
            }

            args.TeleportSourcePlayer();
            if (args.weaponType == WeaponType.Laser) {
                foreach (Vec3U16 pos in args.previous) {
                    args.player.level.Blockchange(pos.X, pos.Y, pos.Z, ExtBlock.Air, true);
                }
                args.previous.Clear();
            } else if (args.previous.Count > 0) {
                Vec3U16 pos = args.previous[0];
                args.previous.RemoveAt(0);
                args.player.level.Blockchange(pos.X, pos.Y, pos.Z, ExtBlock.Air, true);
            }
            task.Repeating = args.previous.Count > 0;
        }
        
        static bool MoveGun(WeaponArgs args) {
            while (true) {
                args.pos = args.PosAt(args.iterations);
                args.iterations++;
                Vec3U16 pos = args.pos;

                ExtBlock cur = args.player.level.GetBlock(pos.X, pos.Y, pos.Z);
                if (cur.IsInvalid) return false;
                if (cur.BlockID != Block.Air && !args.allBlocks.Contains(pos) && HandlesHitBlock(args.player, cur, args.weaponType, pos, true))
                    return false;

                args.player.level.Blockchange(pos.X, pos.Y, pos.Z, args.block);
                args.previous.Add(pos);
                args.allBlocks.Add(pos);
                if (HitsPlayer(args, pos)) return false;

                if (args.iterations > 12 && args.weaponType != WeaponType.Laser) {
                    pos = args.previous[0];
                    args.previous.RemoveAt(0);
                    args.player.level.Blockchange(pos.X, pos.Y, pos.Z, ExtBlock.Air, true);
                }
                
                if (args.weaponType != WeaponType.Laser) return true;
            }
        }
        
        static Vec3U16 MakePos(Player p) { return (Vec3U16)p.Pos.BlockCoords; }
        
        static bool HitsPlayer(WeaponArgs args, Vec3U16 pos) {
            Player pl = GetPlayer(args.player, pos, true);
            if (pl == null) return false;
            
            ExtBlock stone = (ExtBlock)Block.Cobblestone;
            Player p = args.player;
            if (p.level.physics >= 3 && args.weaponType >= WeaponType.Explode) {
                pl.HandleDeath(stone, " was blown up by " + p.ColoredName, true);
            } else {
                pl.HandleDeath(stone, " was shot by " + p.ColoredName);
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Gun [at end]");
            Player.Message(p, "%HAllows you to fire bullets at people");
            Player.Message(p, "%HAvailable [at end] types: %Sexplode, destroy, laser, tp");
        }
    }
}
