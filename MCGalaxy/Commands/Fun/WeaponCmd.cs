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
    public abstract class WeaponCmd : Command {
        
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected abstract string Weapon { get; }
        
        public override void Use(Player p, string message) {
            if (!p.level.Config.Guns) {
                Player.Message(p, Weapon + "s cannot be used on this map!"); return;
            }
            if (p.Game.hasflag != null) {
                Player.Message(p, "You can't use a " + Weapon.ToLower() + " while you have the flag!"); return;
            }

            if (p.aiming && message == "") {
                p.aiming = false;
                p.ClearBlockchange();
                Player.Message(p, "Disabled " + Weapon.ToLower() );
                return;
            }

            WeaponType weaponType = GetWeaponType(p, message);
            if (weaponType == WeaponType.Invalid) return;
            
            p.blockchangeObject = weaponType;
            p.ClearBlockchange();
            p.Blockchange += PlacedMark;

            Player.Message(p, Weapon + " mode engaged, fire at will");
            if (p.aiming) return;

            p.aiming = true;
            AimState state = new AimState();
            state.player = p;
            SchedulerTask task = new SchedulerTask(AimCallback, state, TimeSpan.Zero, true);
            p.CriticalTasks.Add(task);
        }
        
        WeaponType GetWeaponType(Player p, string mode) {
            if (mode == "") return WeaponType.Normal;
            if (mode.CaselessEq("destroy")) return WeaponType.Destroy;
            if (mode.CaselessEq("tp") || mode.CaselessEq("teleport")) return WeaponType.Teleport;
            if (mode.CaselessEq("explode")) return WeaponType.Explode;
            if (mode.CaselessEq("laser")) return WeaponType.Laser;
            
            Help(p);
            return WeaponType.Invalid;
        }
        
        class AimState {
            public Player player;
            public Position oldPos = default(Position);
            public List<Vec3U16> lastGlass = new List<Vec3U16>();
            public List<Vec3U16> glassCoords = new List<Vec3U16>();
        }
        
        static void AimCallback(SchedulerTask task) {
            AimState state = (AimState)task.State;
            Player p = state.player;
            if (state.player.aiming) { DoAim(state); return; }
            
            foreach (Vec3U16 cP in state.lastGlass) {
                if (!p.level.IsValidPos(cP)) continue;
                p.RevertBlock(cP.X, cP.Y, cP.Z);
            }
            task.Repeating = false;
        }
        
        static void DoAim(AimState state) {
            Player p = state.player;
            Vec3F32 dir = DirUtils.GetFlatDirVector(p.Rot.RotY, p.Rot.HeadX);
            ushort x = (ushort)Math.Round(p.Pos.BlockX + dir.X * 3);
            ushort y = (ushort)Math.Round(p.Pos.BlockY + dir.Y * 3);
            ushort z = (ushort)Math.Round(p.Pos.BlockZ + dir.Z * 3);

            int signX = Math.Sign(dir.X) >= 0 ? 1 : -1, signZ = Math.Sign(dir.Z) >= 0 ? 1 : -1;
            CheckTile(p.level, state.glassCoords, x, y, z);
            CheckTile(p.level, state.glassCoords, x + signX, y, z);
            CheckTile(p.level, state.glassCoords, x, y, z + signZ);
            CheckTile(p.level, state.glassCoords, x + signX, y, z + signZ);

            // Revert all glass blocks now not in the ray from the player's direction
            for (int i = 0; i < state.lastGlass.Count; i++) {
                Vec3U16 cP = state.lastGlass[i];
                if (state.glassCoords.Contains(cP)) continue;
                
                if (p.level.IsValidPos(cP))
                    p.RevertBlock(cP.X, cP.Y, cP.Z);
                state.lastGlass.RemoveAt(i); i--;
            }

            // Place the new glass blocks that are in the ray from the player's direction
            foreach (Vec3U16 cP in state.glassCoords) {
                if (state.lastGlass.Contains(cP)) continue;
                state.lastGlass.Add(cP);
                p.SendBlockchange(cP.X, cP.Y, cP.Z, (ExtBlock)Block.Glass);
            }
            state.glassCoords.Clear();
        }
        
        static void CheckTile(Level lvl, List<Vec3U16> glassCoords, int x, int y, int z) {
            Vec3U16 pos;
            if (lvl.IsAirAt(x, y - 1, z)) {
                pos.X = (ushort)x; pos.Y = (ushort)(y - 1); pos.Z = (ushort)z;
                glassCoords.Add(pos);
            }
            if (lvl.IsAirAt(x, y, z)) {
                pos.X = (ushort)x; pos.Y = (ushort)y; pos.Z = (ushort)z;
                glassCoords.Add(pos);
            }
        }
        
        protected abstract void PlacedMark(Player p, ushort x, ushort y, ushort z, ExtBlock block);
        
        
        protected class WeaponArgs {
            public Player player;
            public ExtBlock block;
            public WeaponType weaponType;
            public Vec3U16 pos, start;
            public Vec3F32 dir;
            public bool moving = true;
            
            public List<Vec3U16> previous = new List<Vec3U16>();
            public List<Vec3U16> allBlocks = new List<Vec3U16>();
            public List<Vec3S32> buffer = new List<Vec3S32>();
            public int iterations;
            
            public Vec3U16 PosAt(int i) {
                Vec3U16 target;
                target.X = (ushort)Math.Round(start.X + (double)(dir.X * i));
                target.Y = (ushort)Math.Round(start.Y + (double)(dir.Y * i));
                target.Z = (ushort)Math.Round(start.Z + (double)(dir.Z * i));
                return target;
            }
            
            public void TeleportSourcePlayer() {
                if (weaponType != WeaponType.Teleport) return;
                weaponType = WeaponType.Normal;
                
                int index = previous.Count - 3;
                if (index >= 0 && index < previous.Count) {
                    Vec3U16 coords = previous[index];
                    Position pos = new Position(coords.X * 32, coords.Y * 32 + 32, coords.Z * 32);
                    player.SendPos(Entities.SelfID, pos, player.Rot);
                }
            }
        }
        
        
        protected static Player GetPlayer(Player p, Vec3U16 pos, bool skipSelf) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != p.level) continue;
                if (p == pl && skipSelf) continue;
                
                if (Math.Abs(pl.Pos.BlockX - pos.X) <= 1
                    && Math.Abs(pl.Pos.BlockY - pos.Y) <= 1
                    && Math.Abs(pl.Pos.BlockZ - pos.Z) <= 1)
                {
                    return pl;
                }
            }
            return null;
        }
        
        protected static bool HandlesHitBlock(Player p, ExtBlock block, WeaponType ending, Vec3U16 pos, bool doExplode) {
            if (p.level.physics < 2 || ending == WeaponType.Teleport || ending == WeaponType.Normal) return true;
            
            if (ending == WeaponType.Destroy) {
                bool fireKills = block.BlockID != Block.Air && p.level.BlockProps[block.Index].LavaKills;
                if ((!fireKills && !Block.NeedRestart(block.BlockID)) && block.BlockID != Block.Glass) {
                    return true;
                }
            } else if (p.level.physics >= 3) {
                if (block.BlockID != Block.Glass && doExplode) {
                    p.level.MakeExplosion(pos.X, pos.Y, pos.Z, 1);
                    return true;
                }
            } else {
                return true;
            }
            return false;
        }

        protected enum WeaponType { Invalid, Normal, Destroy, Teleport, Explode, Laser };
    }
}
